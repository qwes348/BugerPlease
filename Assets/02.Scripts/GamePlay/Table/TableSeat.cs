using UnityEngine;

public class TableSeat : MonoBehaviour
{
    private Define.SeatState currentSeatState;
    
    public Define.SeatState CurrentSeatState => currentSeatState;
    public TableSet MyTable { get; set; }

    public void SetSeatState(Define.SeatState newSeatState)
    {
        currentSeatState = newSeatState;
    }

    public void OnCustomerArrived(CustomerController customer)
    {
        MyTable.OnCustomerArrived(customer);
    }
}
