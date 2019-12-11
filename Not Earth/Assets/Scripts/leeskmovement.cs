using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class leeskmovement : MonoBehaviour
{
    Vector3 leeskMovement;

    private void Update()
    {
        leeskMovement = transform.position;
        LeeskMoves();
        transform.position = leeskMovement;
    }

    void LeeskMoves()
    {
        leeskMovement.x += 2 * Time.deltaTime;
        leeskMovement.y += 2 * Time.deltaTime;
    }
}
