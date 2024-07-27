using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LobbyPlayer : MonoBehaviour
{
    [SerializeField] private TextMeshPro _playerName;
    [SerializeField] private Renderer _isReadyRenderer;

    private LobbyPlayerData _data;
    private MaterialPropertyBlock _materialPropertyBlock;
    private MaterialPropertyBlock _materialBlock;

    private void Start()
    {
        _materialPropertyBlock = new MaterialPropertyBlock();
        _materialBlock = new MaterialPropertyBlock();
        Transform body = transform.Find("Body");
        Renderer BodyRender = body.GetComponent<Renderer>();
        BodyRender.GetPropertyBlock(_materialBlock);
        _materialBlock.SetColor("_Color",_data.LobbyColor);
        BodyRender.SetPropertyBlock(_materialBlock);
    }
    public void SetData(LobbyPlayerData data)
    {
        _data = data;
        _playerName.text = _data.Gamertag;

        if (_data.IsReady)
        {
            if (_isReadyRenderer != null)
            {
                _isReadyRenderer.GetPropertyBlock(_materialPropertyBlock);
                _materialPropertyBlock.SetColor("_Color", Color.green);
                _isReadyRenderer.SetPropertyBlock(_materialPropertyBlock);
            }
        }

        gameObject.SetActive(true);
    }
}
