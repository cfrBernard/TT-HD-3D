using System.Collections;

public interface IPlayerController
{
    IEnumerator TakeTurn(Player player, BoardManager board);
}
