using System.Collections;
using System.Collections.Generic;

namespace TheCrew.Shared;

public record CommunicationCard(IPlayCard Card, CommunicationToken Token);

public class PlayerHand : IReadOnlyCollection<IPlayCard>
{
    readonly List<IPlayCard> _cards = new();

    public PlayerHand()
    {
    }

    public int Count => _cards.Count;

    public void AddCards(IEnumerable<IPlayCard> cards)
    {
        _cards.AddRange(cards);
    }

    IEnumerator<IPlayCard> IEnumerable<IPlayCard>.GetEnumerator() => _cards.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _cards.GetEnumerator();

    public CommunicationCard? CommunicationCard { get; set; }
}
