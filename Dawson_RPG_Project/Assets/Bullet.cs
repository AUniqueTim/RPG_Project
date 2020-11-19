using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private SpriteRenderer bulletSpriteRenderer;

    public void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }
    private void Update()
    {
        if (PlayerMovement.Instance.flipX == true) { bulletSpriteRenderer.flipX = false; }
        else if (PlayerMovement.Instance.flipX == false) { bulletSpriteRenderer.flipX = true; }
    }
}
