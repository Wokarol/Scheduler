using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleOutput : MonoBehaviour
{
    [SerializeField] Canvas canvas = default;
    [Space]
    [SerializeField] Bubble blueprint = default;
    [SerializeField] RectTransform startingLine = default;
    [SerializeField] RectTransform endingLine = default;
    [Space]
    [SerializeField] int allowedChannels = default;
    [SerializeField] float spacing = 100;
    [Space]
    [SerializeField] float speed = default;

    Stack<Bubble> pool = new Stack<Bubble>();
    List<Bubble> bubbles = new List<Bubble>();

    Dictionary<string, int> channels = new Dictionary<string, int>();

    public void AddBubble(string channel) {
        AddBubble(channel, channel);
    }

    public void AddBubble(string channel, string text) {
        Bubble bubble = GetBubble().Construct(text);
        bubble.transform.localPosition = new Vector2(startingLine.localPosition.x, GetChannelHeight(GetChannel(channel)));
        bubbles.Add(bubble);
    }

    public void ReserveBubble(string channel) {
        GetChannel(channel);
    }

    int GetChannel(string channel) {
        if (!channels.ContainsKey(channel))
            channels.Add(channel, channels.Count);
        return channels[channel];
    }

    Bubble GetBubble() {
        if(pool.Count > 0) {
            return pool.Pop();
        } else {
            return Instantiate(blueprint, transform);
        }
    }

    Bubble CacheBubble() {
        Bubble item = Instantiate(blueprint, transform);
        pool.Push(item);
        return item;
    }

    void RemoveBubble(Bubble bubble) {
        pool.Push(bubble);
    }

    private void Start() {
        for (int i = 0; i < 20; i++) {
            CacheBubble().transform.localPosition = endingLine.localPosition;
        }
    }

    private void Update() {
        for (int i = bubbles.Count - 1; i >= 0; i--) {
            Bubble bubble = bubbles[i];
            var pos = bubble.transform.localPosition;
            pos -= Vector3.right * ( speed * Time.deltaTime );
            bubble.transform.localPosition = pos;

            if (bubble.transform.localPosition.x < endingLine.localPosition.x) {
                RemoveBubble(bubble);
                bubbles.RemoveAt(i);
            }
        }
    }


    private float GetChannelHeight(int id) {
        var top = allowedChannels - (allowedChannels + 1) * 0.5f;
        var h = top - id;
        return h * spacing;
    }

    private void OnValidate() {
        allowedChannels = Mathf.Max(1, allowedChannels);
    }

    private void OnDrawGizmosSelected() {
        if (canvas == null || startingLine == null || endingLine == null) return;

        var height = (canvas.transform as RectTransform).sizeDelta.y;
        float startX = startingLine.localPosition.x;
        float endX = endingLine.localPosition.x;

        DrawLocalLine(
            new Vector2(startX, height * .5f),
            new Vector2(startX, height * -.5f),
            transform,
            Color.green);

        DrawLocalLine(
           new Vector2(endX, height * .5f),
           new Vector2(endX, height * -.5f),
           transform,
           Color.red);

        for (int i = 0; i < allowedChannels; i++) {
            float cH = GetChannelHeight(i);

            DrawLocalLine(
                new Vector2(startX, cH),
                new Vector2(endX, cH),
                transform,
                Color.white);
        }

    }

    public void DrawLocalLine(Vector3 a, Vector3 b, Transform t, Color color) {
        var localA = transform.TransformPoint(a);
        var localB = transform.TransformPoint(b);
        Debug.DrawLine(localA, localB, color);
    }
}
