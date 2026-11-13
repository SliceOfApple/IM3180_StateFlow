using System;
using UnityEngine;
using static Utility;

public class Arrow : MonoBehaviour
{
    private double oneMeasureDelay;
    private double distanceFromCenter;
    public Vector3 startPos, endPos, actualEndPos;
    public double speed;
    public double timestamp;

    public ArrowDirection arrowDirection;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = gameObject.transform.position;
        Vector3 screenPos = new Vector3(0, Screen.height/2, Camera.main.nearClipPlane);
        endPos = Camera.main.ScreenToWorldPoint(screenPos);
        endPos.x = startPos.x;
        endPos.z = 0f;

        actualEndPos = endPos;
        oneMeasureDelay = MusicManager.Instance.secPerBeat * 4;

        //speed = Vector3.Distance(startPos, endPos) / (timestamp - MusicManager.instance.GetElapsedTime());
        speed = Vector3.Distance(startPos, endPos) / oneMeasureDelay;

        endPos.y += -2; // After calculating the speed required, just make the arrows overshoot to signify misses.
    }

    // Update is called once per frame
    void Update()
    {
        if (MusicManager.Instance == null || GameManager.Instance == null)
        {
            return;
        }
        float step = (float)speed * Time.deltaTime;

        //Debug.Log(distanceFromCenter);
        transform.position = Vector3.MoveTowards(transform.position, endPos, step);

        if (MusicManager.Instance.ElapsedTime > timestamp + 0.5*MusicManager.Instance.secPerBeat)
        {
            //GameManager.instance.arrowQueue.Remove(gameObject);
            switch (arrowDirection)
            {
                case ArrowDirection.LEFT:
                    GameManager.Instance.arrows_left.RemoveAt(0);
                    break;
                case ArrowDirection.RIGHT:
                    GameManager.Instance.arrows_right.RemoveAt(0);
                    break;
                case ArrowDirection.UP:
                    GameManager.Instance.arrows_up.RemoveAt(0);
                    break;
                case ArrowDirection.DOWN:
                    GameManager.Instance.arrows_down.RemoveAt(0);
                    break;
            }
            GameManager.Instance.UpdateScore(HitAccuracy.MISS, arrowDirection, transform.position);
            Destroy(gameObject);
            Debug.Log("Miss");
        }
    }
}