using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    // Component references
    Rigidbody rb;  // Unity's physics component

    [SerializeField] float speed = 5f;
    [SerializeField] float jumpHeight = 5f;
    [SerializeField] float respawnHeight = -10f;

    //keep track of current horizontal direction
    Vector2 direction = Vector2.zero;
    private Vector3 originalScale;

    //keep track of if the player is on the ground
    bool isGrounded = false;
    bool isShiftPressed = false; 

    // Start is called before the first frame update
    void Start()
    {
        // Get references to the components attached to the current GameObject
        rb = GetComponent<Rigidbody>();
        originalScale = rb.transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        //if player is below respawn height, respawn it
        if (transform.position.y < respawnHeight)
            Respawn();
        Move(direction.x, direction.y);
        // Check if the Shift key is pressed down
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            isShiftPressed = true;
            PerformShiftAction();
        }

        // Optionally, check if the Shift key is released
        if (isShiftPressed && (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift)))
        {
            isShiftPressed = false;
            RevertShiftAction();
        }
    }

    private void PerformShiftAction()
    {
        rb.transform.localScale = new Vector3(originalScale.x * 2, originalScale.y / 2, originalScale.z * 2);


    }

    private void RevertShiftAction()
    {
        // Revert to original scale
        rb.transform.localScale = originalScale;
    }

    void OnJump()
    {
        //if player is on the ground, jump
        if (isGrounded)
            Jump();
    }

    private void Jump()
    {
        // Set the y velocity to some positive value while keeping the x and z whatever they were originally
        rb.velocity = new Vector3(rb.velocity.x, jumpHeight, rb.velocity.z);
    }

    void OnMove(InputValue moveVal)
    {
        //store input as a 2D vector
        Vector2 direction = moveVal.Get<Vector2>();
        this.direction = direction;
    }

    private void Move(float x, float z)
    {
        // Set the x & z velocity of the Rigidbody to correspond with our inputs while keeping the y velocity what it originally is.
        rb.velocity = new Vector3(x * speed, rb.velocity.y, z * speed);
    }

    //commonly used function but not used in this case
    void OnCollisionEnter(Collision collision)
    {

    }

    void OnCollisionStay(Collision collision)
    {
        //check if angle between normal vector of object of contact and up vector is less than 45 degrees
        //AKA if-statement is true if player is touching another object that is 0 to 45 degrees slope
        if (Vector3.Angle(collision.GetContact(0).normal, Vector3.up) < 45f)
            isGrounded = true;
        else
            isGrounded = false;
    }

    void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }

    private void Respawn()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
