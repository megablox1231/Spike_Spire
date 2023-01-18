using UnityEngine;
using System.Collections;

//[RequireComponent (typeof (BoxCollider2D))] cant use because need colliders as children in spike spire
public class RaycastController : MonoBehaviour {

	public LayerMask collisionMask; //colliders that are interacted with by the raycasts
    public LayerMask SwordJumpMask; //colliders that can be hit by the jump collider
    public LayerMask SlashMask;     //colliders that can be hit by the FowardSlash collider
    public LayerMask NoSlashMask;   //colliders that Cannot be hit by the FowardSlash collider
    public LayerMask brittleSBMask;
    public LayerMask deadlySBMask;

    public BoxCollider2D jumpCollider;
    public BoxCollider2D slashCollider;

    [HideInInspector]
    public ContactFilter2D brittleContact;


    public const float skinWidth = .015f; //TODO refine raycount
    const float dstBetweenRays = .05f; //make smaller for smaller sizes
	[HideInInspector]
	public int horizontalRayCount;
	[HideInInspector]
	public int verticalRayCount;

	[HideInInspector]
	public float horizontalRaySpacing;
	[HideInInspector]
	public float verticalRaySpacing;

	[HideInInspector]
    public BoxCollider2D collider;
	public RaycastOrigins raycastOrigins;

	public virtual void Awake() {
        collider = transform.Find("Player Collider").GetComponent<BoxCollider2D>();//if collider is not part of this gameobject, make hidden and initialize here
	}

	public virtual void Start() {
		CalculateRaySpacing ();

        brittleContact = new ContactFilter2D();
        brittleContact.SetLayerMask(brittleSBMask);
        brittleContact.useTriggers = true;
	}

	public void UpdateRaycastOrigins() {
		Bounds bounds = collider.bounds;
		bounds.Expand (skinWidth * -2);
		
		raycastOrigins.bottomLeft = new Vector2 (bounds.min.x, bounds.min.y);
		raycastOrigins.bottomRight = new Vector2 (bounds.max.x, bounds.min.y);
		raycastOrigins.topLeft = new Vector2 (bounds.min.x, bounds.max.y);
		raycastOrigins.topRight = new Vector2 (bounds.max.x, bounds.max.y);
	}
	
	public void CalculateRaySpacing() {
		Bounds bounds = collider.bounds;
		bounds.Expand (skinWidth * -2); //shrunk

		float boundsWidth = bounds.size.x;
		float boundsHeight = bounds.size.y;
		
		horizontalRayCount = Mathf.RoundToInt (boundsHeight / dstBetweenRays);
		verticalRayCount = Mathf.RoundToInt (boundsWidth / dstBetweenRays);
		
		horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
		verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
	}
	
	public struct RaycastOrigins {
		public Vector2 topLeft, topRight;
		public Vector2 bottomLeft, bottomRight;
	}
}
