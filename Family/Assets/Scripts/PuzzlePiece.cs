using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzlePiece : MonoBehaviour
{
    private Slot _slot;
    private SpriteRenderer _spriteRenderer;
    private PersonCode _personCode;
    private bool _dragging = false;
    private bool _placed = false;
    private Vector2 _offset, _originalPosition;
    public enum PersonCode : int
    {
        Elad = 0,
        Merav = 1,
        Oren = 2,
        Tavor = 3,
        Tom = 4,
        Hana = 5,
        Yosi = 6,
        Ronit = 7,
        Doron = 8,
    }
    [SerializeField] private AudioClip successSound;
    [SerializeField] private AudioClip failSound;
    private AudioClip _nameSound;
    private AudioSource _audioSource;

    public void Init(int personInd)
    {
        _personCode = (PersonCode)personInd;
        LoadResourcesFromFile(_personCode);
    }

    void Awake()
    {
        _originalPosition = transform.position;
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        _audioSource = gameObject.GetComponent<AudioSource>();
    }

    void Update()
    {
        if (_placed) return;
        if (!_dragging) return;

        Vector2 mousePosition = GetMousePos();
        transform.position = mousePosition - _offset;
    }

    void OnMouseDown()
    {
        _dragging = true;
        _offset = GetMousePos() - (Vector2)transform.position;
    }

    bool IsPlacedInSlot()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.zero);
        if (hit.collider != null)
        {
            Slot slot = hit.transform.gameObject.GetComponent<Slot>();
            if (slot != null)
            {
                return true;
            }
        }
        return false;
    }

    bool isCorrectSlot(Slot slot)
    {
        return slot.GetCurrentPersonCode() == _personCode;
    }

    void OnMouseUp()
    {
        if (IsPlacedInSlot() && isCorrectSlot(_slot))
        {
            AudioSource.PlayClipAtPoint(successSound, transform.position);
            _placed = true;
            Destroy(gameObject);
            return;
        }
        else
        {
            _dragging = false;
            if (IsPlacedInSlot())
            {
                AudioSource.PlayClipAtPoint(failSound, transform.position);
                transform.position = _originalPosition;
            }
        }
    }

    private Vector2 GetMousePos()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void LoadResourcesFromFile(PersonCode personCode)
    {
        string spriteFilename = "Sprites/";
        string audioFilename = "Audio/";
        switch (personCode)
        {
            case (PersonCode.Elad):
                spriteFilename += "Face_Elad";
                audioFilename += "elad";
                break;
            case (PersonCode.Merav):
                spriteFilename += "Face_Merav";
                audioFilename += "merav";
                break;
            case (PersonCode.Oren):
                spriteFilename += "Face_Oren";
                audioFilename += "oren";
                break;
            case (PersonCode.Tavor):
                spriteFilename += "Face_Tavor";
                audioFilename += "tavor";
                break;
            case (PersonCode.Tom):
                spriteFilename += "Face_Tom";
                audioFilename += "tom";
                break;
            case (PersonCode.Hana):
                spriteFilename += "Face_Hana";
                audioFilename += "hana";
                break;
            case (PersonCode.Yosi):
                spriteFilename += "Face_Yosi";
                audioFilename += "yosi";
                break;
            case (PersonCode.Ronit):
                spriteFilename += "Face_Ronit";
                audioFilename += "ronit";
                break;
            case (PersonCode.Doron):
                spriteFilename += "Face_Doron";
                audioFilename += "doron";
                break;
        }
        Debug.Log("Loading Sprite: " + spriteFilename);
        Debug.Log("Loading Audio: " + audioFilename);
        Sprite sprite = Resources.Load<Sprite>(spriteFilename);
        AudioClip audioClip = Resources.Load<AudioClip>(audioFilename);
        _audioSource.clip = audioClip;
        _spriteRenderer.sprite = sprite;
        _nameSound = audioClip;
    }

    public PersonCode GetPersonCode()
    {
        return _personCode;
    }

    public void SetSlot(Slot obj)
    {
        _slot = obj;
    }

    public void PlayNameSound(){
        _audioSource.clip = _nameSound;
        _audioSource.PlayDelayed(0.9f);
        // AudioSource.PlayClipAtPoint(_nameSound, transform.position, 1.0f);
    }
}
