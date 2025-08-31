using System.Collections;
using UnityEngine;

public class AIController : IPlayerController
{
    public IEnumerator TakeTurn(Player player, BoardManager board)
    {
        Debug.Log($"[Turn] {player.Name}'s turn (AI)");

        // simuler réflexion
        float waitTime = 0f;
        waitTime = Random.Range(2.5f, 4.5f);
        yield return new WaitForSeconds(waitTime);

        // carte de la main aléatoire
        if (player.Hand.Count == 0)
        {
            Debug.LogWarning("AI has no cards to play!");
            yield break;
        }

        Card card = player.Hand[Random.Range(0, player.Hand.Count)];

        // Choisit un slot vide aléatoire
        for (int attempt = 0; attempt < 20; attempt++)
        {
            int x = Random.Range(0, BoardManager.SIZE);
            int y = Random.Range(0, BoardManager.SIZE);

            if (board.TryPlaceCard(x, y, card))
            {
                // résoudre captures
                Debug.Log($"[AI] {player.Name} placed {card.Data.name} at {x},{y}");
                break;
            }
        }

        yield return new WaitForSeconds(0.2f);
    }
}
