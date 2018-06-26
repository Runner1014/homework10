using UnityEngine;
using UnityEngine.Networking;

public class PlayerMove : NetworkBehaviour
{
    public GameObject bulletPrefab;

    public override void OnStartLocalPlayer()
    {
        MeshRenderer[] a = this.transform.GetComponentsInChildren<MeshRenderer>();
        foreach (var b in a)
        {
            b.material.color = Color.red;
        }
    }

    void Update()
    {
        if (!isLocalPlayer)
            return;

        var x = Input.GetAxis("Horizontal") * 5f;
        var z = Input.GetAxis("Vertical") * 0.5f;
        transform.Translate(0, 0, z);
        transform.Rotate(new Vector3(0, x, 0));

        Transform camera = Camera.main.transform;
        camera.position = new Vector3(transform.position.x, 10, transform.position.z) - transform.forward * 12;
        camera.rotation = transform.localRotation;
        camera.Rotate(20, 0, 0);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Command function is called from the client, but invoked on the server
            CmdFire();
        }
    }

    [Command]
    void CmdFire()
    {
        // This [Command] code is run on the server!

        // create the bullet object locally
        var bullet = (GameObject)Instantiate(
            bulletPrefab,
            transform.position + transform.forward * 2 + transform.up * 1.4f,
            transform.localRotation);

        bullet.GetComponent<Rigidbody>().velocity = transform.forward * 40;

        // spawn the bullet on the clients
        NetworkServer.Spawn(bullet);

        // when the bullet is destroyed on the server it will automaticaly be destroyed on clients
        Destroy(bullet, 2.0f);
    }
}

