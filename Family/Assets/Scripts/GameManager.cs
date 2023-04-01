using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int numPiecesToSpawn;
    [SerializeField] private Slot _slot;
    [SerializeField] private PuzzlePiece _piecePrefab;
    [SerializeField] private Transform _pieceParent; // All puzzle pieces are going to be added as children to this object
    [SerializeField] private int _numOfPieces;
    [SerializeField] private TMPro.TMP_Text personText;
    [SerializeField] private TMPro.TMP_Text startText;
    [SerializeField] private AudioClip gameOverSound;
    private int _lastChildCount;
    private bool _gameEnded;

    private void UpdateText(string text)
    {
        personText.SetText(text);
    }

    void Update(){
        if (_gameEnded) return;
        if (_pieceParent.childCount < _lastChildCount){
            SelectPiece();
        }
    }

    public void StartGame(){
        _gameEnded = false;
        _numOfPieces = (int)System.Enum.GetValues(typeof(PuzzlePiece.PersonCode)).Cast<PuzzlePiece.PersonCode>().Last();
        Spawn();
        SelectPiece();
        startText.gameObject.SetActive(false);
    }

    private void Spawn()
    {
        // Randomly select images and spawn them
        List<int> numberList = Enumerable.Range(0, _numOfPieces).ToList();
        List<int> randomSet = numberList.OrderBy(s => Random.value).Take(numPiecesToSpawn).ToList();
        for (int i = 0; i < randomSet.Count; i++)
        {
            Vector3 randomWorldPoint = Camera.main.ScreenToWorldPoint(RandomScreenCoordinate());
            randomWorldPoint.z = 0;
            var spawnedPiece = Instantiate(_piecePrefab, randomWorldPoint, Quaternion.identity);
            spawnedPiece.transform.SetParent(_pieceParent);
            spawnedPiece.name = "PuzzlePiece:" + randomSet[i].ToString();
            spawnedPiece.Init(randomSet[i]);
            spawnedPiece.SetSlot(_slot);
        }
        _lastChildCount = numPiecesToSpawn;
    }

    private Vector2 RandomScreenCoordinate(float distFromCenter=0.0f, float edge=0.0f){
        float x = Random.Range(edge, 0.5f * Screen.width - edge) * Random.Range(1, 3);
        float y = Random.Range(edge, 0.5f * Screen.height - edge) * Random.Range(1, 3);
        Vector2 xy = new Vector2(x, y);
        return xy;
    }

    void SelectPiece()
    {
        if (_pieceParent.childCount <= 0 && !_gameEnded)
        {
            EndGame();
        }
        else
        {
            // Select one of the remaining names
            PuzzlePiece piece = _pieceParent.GetChild(0).GetComponent<PuzzlePiece>();
            PuzzlePiece.PersonCode personCode = piece.GetPersonCode();
            UpdateText(personCode.ToString());
            piece.PlayNameSound();
            _slot.SetCurrentPersonCode(personCode);
            _lastChildCount = _pieceParent.childCount;
        }
    }

    void EndGame()
    {
        Debug.Log("Game Ended!");
        AudioSource.PlayClipAtPoint(gameOverSound, transform.position);
        _gameEnded = true;
        startText.gameObject.SetActive(true);
        UpdateText("Game Over");
    }
}
