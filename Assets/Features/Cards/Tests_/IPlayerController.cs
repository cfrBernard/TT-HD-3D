using System.Collections;

public interface IPlayerController
{
    IEnumerator TakeTurn(Player player, BoardManager board);

    void BeginMulligan(Player player, System.Action onDone);
    void CancelMulligan(Player player);
}
