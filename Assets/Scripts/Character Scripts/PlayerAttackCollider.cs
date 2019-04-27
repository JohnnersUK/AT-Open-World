using UnityEngine;

public class PlayerAttackCollider : MonoBehaviour
{
    // Components
    private Animator anim;
    private SphereCollider col;
    private GameObject player;

    private bool hit = false;

    // Use this for initialization
    void Start()
    {
        col = GetComponent<SphereCollider>();

        player = transform.root.gameObject;
        anim = player.GetComponent<Animator>();

        col.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (anim.GetBool("Attacking"))
        {
            if (hit == false)
            {
                col.enabled = true;
            }
        }
        else
        {
            col.enabled = false;
            hit = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "character")
        {
            Debug.Log("Hit " + other.name);

            other.GetComponent<Animator>().Play("Base Layer.Combat.Hit.Hit " + UnityEngine.Random.Range(0, 4));

            col.enabled = false;
            hit = true;
        }
    }
}
