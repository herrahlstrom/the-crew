using System.Runtime.CompilerServices;

namespace TheCrew.Shared;

public enum ValueCardSuit
{
   Green, Pink, Blue, Yellow, Rocket
}

public enum MissionCardToken
{
   None,
   First, Second, Third, Fourth, Fifth,
   FirstFlex, SecondFlex, ThirdFlex,
   Last
}

public interface ICard { }
public interface IMissionTask { bool Completed { get; set; } }

public interface IValueCard : ICard { ValueCardSuit Suit { get; } int Value { get; } }

public interface IGenericMissionTask : IMissionTask
{
}

public interface IPlayCard : IValueCard, ICard { }
public interface IMissionTaskCard : IMissionTask, ICard { };

public record PlayCard(ValueCardSuit Suit, int Value) : IPlayCard
{
   public override string ToString() => $"{Suit} {Value}";
}
public record ValueMissionCardTask(ValueCardSuit Suit, int Value, MissionCardToken Token) : IMissionTaskCard
{
   public bool Completed { get; set; }
   public override string ToString() => $"{Suit} {Value}";
}
