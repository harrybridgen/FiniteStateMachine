class Program {
    static void Main(String[] args) {
        VendingMachine vendingMachine = new VendingMachine();
        vendingMachine.InsertCoin(20);
        vendingMachine.InsertCoin(50);
        vendingMachine.InsertCoin(20);
        vendingMachine.InsertCoin(22);
        vendingMachine.InsertCoin(20);
        vendingMachine.InsertCoin(20);
    }
}

public abstract class State {
    protected VendingMachine machine;
    public void SetMachine(VendingMachine machine) { this.machine = machine; }
    public virtual void OnEnter() { }
    public virtual void InsertCoin(int amount) {
        if (!machine.IsValidAmount(amount)) {
            Console.WriteLine("Invalid coin! Returning...");
            return;
        }
    }

}

public class InsertCoinState : State {
    public InsertCoinState(VendingMachine machine) {
        base.SetMachine(machine);
    }
    public override void OnEnter() {
        Console.WriteLine("The vending machine is asking for money!");
    }

    public override void InsertCoin(int amount) {
        if (!machine.IsValidAmount(amount)) {
            Console.WriteLine("Invalid coin? 10, 20, 50 or 100!");
            return;
        }
        if (machine.GetItemStock() <= 0) {
            Console.WriteLine("Out of stock!");
            return;
        }
        machine.AddAmount(amount);
        Console.WriteLine("You insert " + amount);

        if (machine.GetAmount() >= machine.GetItemPrice()){
            machine.ChangeState(new DispenseState(machine));
        }
    }
}

public class DispenseState : State {
    public DispenseState(VendingMachine machine) {
        base.SetMachine(machine);
    }
    public override void OnEnter() {
        Console.WriteLine("The vending machine gives you an item!");
        Console.WriteLine("Remaining stock: " + machine.GetItemStock());
        machine.AddAmount(-machine.GetItemPrice());
        machine.DecrementItemStock();
        machine.ChangeState(new GiveChangeState(machine));
    }

    public override void InsertCoin(int amount) {
        base.InsertCoin(amount);
        Console.WriteLine("Machine is busy... Returning " + amount);
    }
}

public class GiveChangeState : State {
    public GiveChangeState(VendingMachine machine) {
        base.SetMachine(machine);
    }
    public override void OnEnter() {
        if (machine.GetAmount() > 0) {
            Console.WriteLine("Vending Machine gives you " + machine.GetAmount() + " back");
            machine.AddAmount(-machine.GetAmount());
        }
        machine.ChangeState(new InsertCoinState(machine));
    }

    public override void InsertCoin(int amount) {
        base.InsertCoin(amount);
        Console.WriteLine("Machine is busy... Returning " + amount);
    }
}

public class VendingMachine {

    public VendingMachine() {
        this.ChangeState(new InsertCoinState(this));
    }
    private State currentState;
    private int currentAmount = 0;
    private int itemPrice = 50;
    private int itemStock = 2;
    public int GetAmount() { return this.currentAmount; }
    public void AddAmount(int amount){ this.currentAmount += amount; }
    public int GetCurrentAmount() { return currentAmount; }
    public int GetItemPrice() { return itemPrice; }
    public int GetItemStock() { return itemStock; }
    public int DecrementItemStock() { return itemStock -= 1; }
    public Boolean IsValidAmount(int amount) {
        return amount == 10 || amount == 20 || amount == 50 || amount == 100;
    }

    public void InsertCoin(int amount) {
        currentState.InsertCoin(amount);
    }

    public void ChangeState(State newState) {

        this.currentState = newState;
        this.currentState.OnEnter();
    }

}

