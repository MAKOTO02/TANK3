using UnityEngine;

public class PlayerDesc : Singleton<PlayerDesc>
{
    public string bulletType { get; set; }

    protected override void Awake()
    {
    }

    private void Update()
    {
    }
}
