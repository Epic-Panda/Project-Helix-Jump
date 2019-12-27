using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controlls ball behaviour.
/// </summary>
public class BallController : MonoBehaviour
{
    public GameObject hitEffectPref;

    public int cleanBreakNeeded = 3;
    public int breakWorth = 1;

    bool collisionEnter;
    bool triggerEnter;
    int cleanBreak;

    bool jump = false;

    /// <summary>
    /// Define starting values of variables.
    /// </summary>
    public void DefineStart()
    {
        collisionEnter = false;
        triggerEnter = false;
        cleanBreak = 0;
        jump = false;
    }

    /// <summary>
    /// Used to define starting values of variables.
    /// </summary>
    void Start()
    {
        DefineStart();
    }

    /// <summary>
    /// Checking every frame when is time to jump.
    /// </summary>
    void Update()
    {
        if (GameManager.instance.EOG || GameManager.instance.forceQuit || GameManager.instance.levelCleared)
            return;

        if (!collisionEnter)
            return;

        if (jump)
        {
            GetComponent<Rigidbody>().AddForce(new Vector3(0, 6, 0), ForceMode.Impulse);
            jump = false;
        }
    }

    /// <summary>
    /// Determinate what action will be taken based on object that was hit.
    /// It is possible to hit destroy or safe piece of platform. 
    /// Function is using raycast for easier detection of colliosion object since ball is going thru colliders due its speed.
    /// </summary>
    /// <param name="collision">Object that collides with ball.</param>
    private void OnCollisionEnter(Collision collision)
    {
        if (GameManager.instance.EOG || GameManager.instance.forceQuit || GameManager.instance.levelCleared)
            return;

        // enter only once in collision
        if (collisionEnter)
            return;

        collisionEnter = true;

        // set ball on top of platform if ball is not above platform
        if (collision.transform.position.y + .25f > transform.position.y - .1f)
            transform.position = new Vector3(transform.position.x, collision.transform.position.y + .35f, transform.position.z);

        // cast ray to determinate if platform piece is safe, using this instead of collision is safer
        RaycastHit rayHit;
        LayerMask layerMask = 1 << 0;
        Physics.Raycast(transform.position, -transform.up, out rayHit, .5f, layerMask);

        // find out if ray hit safe or destroy piece, if it is escape then check collision object tag to determinate further action
        if (rayHit.transform.tag == "safe" || rayHit.transform.tag == "escape" && collision.transform.tag == "safe")
        {
            jump = true;
        }
        else if (rayHit.transform.tag == "destroy" || rayHit.transform.tag == "escape" && collision.transform.tag == "destroy")
        {
            if (cleanBreak >= cleanBreakNeeded && collision.transform.tag != "undestructable")
                collision.transform.tag = "Untagged";

            // if clean strike is not enough then its game over
            if (cleanBreak < cleanBreakNeeded)
            {
                GameManager.instance.EndOfGame();
                cleanBreak = 0;
                return;
            }
            jump = true;
        }

        GameObject hitEffect = Instantiate(hitEffectPref, transform.position - new Vector3(0, .1f, 0), Quaternion.Euler(-90, 0, 0));
        var psys = hitEffect.GetComponent<ParticleSystem>().main;
        psys.startColor = GameManager.instance.spawnController.ballColor;

        SoundManager.instance.Play("platform hit");

        GameManager.instance.cameraController.target = collision.gameObject;

        if (collision.transform.tag == "undestructable")
        {
            cleanBreak++;
            GameManager.instance.AddScore(breakWorth * cleanBreak);

            GameManager.instance.RefreshProgress();
            GameManager.instance.LevelWon();

            cleanBreak = 0;
            return;
        }

        // check if there is clean hit
        if (cleanBreak >= cleanBreakNeeded)
        {
            triggerEnter = false;

            Transform[] slices = collision.transform.parent.gameObject.GetComponentsInChildren<Transform>();
            foreach (Transform t in slices)
                if (t.gameObject.GetComponent<MeshRenderer>())
                {
                    t.gameObject.GetComponent<MeshRenderer>().material = GameManager.instance.spawnController.cleanHitPlatformMaterial;
                    t.gameObject.GetComponent<MeshRenderer>().material.color = gameObject.GetComponent<MeshRenderer>().material.color;
                }

            collisionEnter = false;
            DestroyPlatform(collision.transform.parent.gameObject);
        }

        cleanBreak = 0;
    }

    /// <summary>
    /// On collision exit is executing action that was determinated in on collision enter function.
    /// </summary>
    /// <param name="collision">Object that collides with ball.</param>
    private void OnCollisionExit(Collision collision)
    {
        collisionEnter = false;
    }

    /// <summary>
    /// On trigger enter is called when ball hits trigger object.
    /// </summary>
    /// <param name="collider">Trigger object that collides with ball.<</param>
    private void OnTriggerEnter(Collider collider)
    {
        if (GameManager.instance.EOG || GameManager.instance.forceQuit || GameManager.instance.levelCleared)
            return;

        triggerEnter = true;
    }

    /// <summary>
    /// When ball exit from collision with trigger, destroy platform that trigger belongs to.
    /// </summary>
    /// <param name="collider">Trigger object that collides with ball.<</param>
    private void OnTriggerExit(Collider collider)
    {
        if (GameManager.instance.EOG || GameManager.instance.levelCleared || GameManager.instance.forceQuit)
            return;

        if (!triggerEnter)
            return;

        collisionEnter = false;
        triggerEnter = false;

        // if ball is under collider then proceed
        if (transform.position.y > collider.transform.position.y)
            return;

        GameManager.instance.startingUIObj.SetActive(false);

        DestroyPlatform(collider.transform.parent.gameObject);
    }

    /// <summary>
    /// Function is destroying parent object of platform piece, adds score and plays sound of destruction.
    /// </summary>
    /// <param name="pieceParent">Parent of platform piece.</param>
    void DestroyPlatform(GameObject pieceParent)
    {
        if (pieceParent.tag != "piece container")
            pieceParent = pieceParent.transform.parent.gameObject;

        SoundManager.instance.Play("platform destroy");

        pieceParent.transform.parent = null;
        GameManager.instance.cameraController.target = gameObject;

        cleanBreak++;
        GameManager.instance.AddScore(breakWorth * cleanBreak);

        GameManager.instance.RefreshProgress();

        Rigidbody[] rbs = pieceParent.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody r in rbs)
        {
            // remove mesh collider first
            List<Collider> colliderList = new List<Collider>();
            colliderList.AddRange(r.gameObject.GetComponents<Collider>());
            colliderList.AddRange(r.gameObject.GetComponentsInChildren<Collider>());

            foreach (Collider c in colliderList)
                Destroy(c);

            r.isKinematic = false;
            r.AddForce((new Vector3(0, .5f, 0) - r.transform.forward.normalized) * 5, ForceMode.Impulse);
        }

        Destroy(pieceParent, 1.5f);
    }
}
