using UnityEngine;

public class TableSeat : MonoBehaviour
{
    private Define.SeatState currentSeatState;
    
    public Define.SeatState CurrentSeatState => currentSeatState;

    public void SetSeatState(Define.SeatState newSeatState)
    {
        currentSeatState = newSeatState;
    }
}
