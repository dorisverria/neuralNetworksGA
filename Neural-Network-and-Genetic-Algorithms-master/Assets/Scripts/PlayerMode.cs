using UnityEngine;
using System.Collections.Generic;

public abstract class PlayerMode : MonoBehaviour
{
    protected Transform trnsfrm;
    protected Rigidbody rigidBody;
    protected Collider col;
    protected Vector3 init;
    protected int score;
    protected int rewardsTaken;
    protected bool isLiving;
    protected bool isHit;
    protected PlayerType playerType;
    
    public virtual void Initialize(PlayerType type)
    {
        trnsfrm = transform;
        rigidBody = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        init = trnsfrm.position;
        playerType = type;
        isHit = false;
        isLiving = true;
    }

    public abstract void ManualUpdate();

    void OnCollisionEnter(Collision colInfo)
    {
        if (colInfo.collider.tag == "Obstacle")
        {
            Dead();
        }
    }

    void OnTriggerEnter(Collider colInfo)
    {
        if (colInfo.gameObject.tag == "YellowBall")
        {
            RewardTaken(colInfo);
        }
    }

    protected void RewardTaken(Collider colInfo)
    {
        rewardsTaken++;
        colInfo.GetComponent<Reward>().DestroyReward();
    }

    protected void Dead()
    {
        rigidBody.velocity = Vector3.zero;
        isHit = true;
        isLiving = false;
        col.enabled = false;
    }

    public virtual void Reset()
    {
        trnsfrm.position = init;
        trnsfrm.eulerAngles = new Vector3(0, 270, 0);
        isLiving = true;
        rewardsTaken = 0;
        col.enabled = true;
    }

    public bool hit
    {
        get { return isHit; }
        set { isHit = value; }
    }

    public Collider collider
    {
        get { return col; }
    }

    public Vector3 position
    {
        get { return trnsfrm.position; }
    }

    public Vector3 velocity
    {
        get { return rigidBody.velocity; }
    }

    public Quaternion rotation
    {
        get { return trnsfrm.rotation; }
    }

    public bool alive
    {
        get { return isLiving; }
    }

    public PlayerType type
    {
        get { return playerType; }
    }

    public int Score
    {
        get { return score; }
    }
}
