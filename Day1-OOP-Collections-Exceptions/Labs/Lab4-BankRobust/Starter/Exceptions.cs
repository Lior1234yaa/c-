namespace Day1.Lab4;

// TODO A: היררכיית חריגות מותאמות:
//   BankException : Exception                      (בסיס)
//   AccountNotFoundException : BankException       (עם property AccountId)
//   InsufficientFundsException : BankException     (עם Requested, Available, Shortfall)
//   InvalidAmountException : BankException         (עם Amount)
// כרגע יש רק מחלקה אחת כללית — החליפו אותה בהיררכיה.
public class BankException(string message) : Exception(message);
