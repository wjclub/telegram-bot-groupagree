namespace GroupAgreeBot.Models;

public sealed class GroupAgreeBotUser {
    public required Guid Id { get; init; }
    public required long TelegramId { get; init; }
    public required string FirstName { get; set; }
    public string? LastName { get; set; }

    // override equals and hashcode
    public override bool Equals(object? obj) {
        if (obj is GroupAgreeBotUser user) {
            return Id == user.Id;
        }
        return false;
    }

    public override int GetHashCode() {
        return HashCode.Combine(Id);
    }

    public static bool operator ==(GroupAgreeBotUser? left, GroupAgreeBotUser? right) {
        return Equals(left, right);
    }

    public static bool operator !=(GroupAgreeBotUser? left, GroupAgreeBotUser? right) {
        return !Equals(left, right);
    }

    public override string ToString() {
        if(LastName is null)
            return FirstName;
        return $"{FirstName} {LastName}";
    }
}