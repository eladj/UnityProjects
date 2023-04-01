using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    private PuzzlePiece.PersonCode _currentPersonCode;

    public void SetCurrentPersonCode(PuzzlePiece.PersonCode personCode){
        _currentPersonCode = personCode;
    }

    public PuzzlePiece.PersonCode GetCurrentPersonCode(){
        return _currentPersonCode;
    }
}
