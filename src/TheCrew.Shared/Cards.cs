using System.Runtime.CompilerServices;

namespace TheCrew.Shared;

public enum ValueCardSuit
{
   Rocket, Green, Pink, Blue, Yellow
}

public enum MissionCardToken
{
   None,
   First, Second, Third, Fourth, Fifth,
   FirstFlex, SecondFlex, ThirdFlex,
   Last
}

public interface ICard { }
public interface IMissionTask { }

public interface IValueCard : ICard { ValueCardSuit Suit { get; } int Value { get; } }

public interface IGenericMissionTask : IMissionTask
{
   bool IsCompleted();
   bool IsImpossible();
}

public interface IPlayCard : IValueCard, ICard { }
public interface IMissionCardTask : IMissionTask, ICard { bool Completed { get; set; } }

public record ValueCard(ValueCardSuit Suit, int Value) : IValueCard
{
   public override string ToString() => $"{Suit} {Value}";
   public bool SameCard(ICard other) => other is ValueCard valueCard && valueCard.Suit == Suit && valueCard.Value == Value;
}
public record PlayCard(ValueCardSuit Suit, int Value) : ValueCard(Suit, Value), IPlayCard
{
   public override string ToString() => $"{Suit} {Value}";
}
public record ValueMissionCardTask(ValueCardSuit Suit, int Value, MissionCardToken Token) : ValueCard(Suit, Value), IMissionCardTask
{
   public bool Completed { get; set; }
   public override string ToString() => $"{Suit} {Value}";
}
