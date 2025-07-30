class Program {
    static void Main(String[] args) {
        VendingMachine vendingMachine = new VendingMachine();
        vendingMachine.InsertCoin(20);
        vendingMachine.InsertCoin(50);
        vendingMachine.InsertCoin(20);
        vendingMachine.InsertCoin(20);
    }
}

public abstract class State {
    protected VendingMachine machine;
    public void SetMachine(VendingMachine machine) { this.machine = machine; }
    public void OnEnter() { }
    public void OnLeave() { }
    public virtual void InsertCoin(int amount) {
        if (!machine.IsValidAmount(amount)) {
            Console.WriteLine("Invalid coin! Returning...");
            return;
        }
        Console.WriteLine("Machine is dispensing... Returning " + amount);
    }

}

public class InsertCoinState : State {
    public InsertCoinState(VendingMachine machine) {
        base.SetMachine(machine);
    }
    public void OnEnter() {
        Console.WriteLine("The vending machine is asking for money!");
    }

    public void OnLeave() {
    }

    public override void InsertCoin(int amount) {
        if (!machine.IsValidAmount(amount)) {
            Console.WriteLine("Invalid coin? 10,20,50 or 100!");
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
    public void OnEnter() {
        Console.WriteLine("The vending machine gives you an item!");
        machine.AddAmount(-machine.GetItemPrice());
        machine.DecrementItemStock();
        machine.ChangeState(new GiveChangeState(machine));
    }

    public void OnLeave() {
    }

    public override void InsertCoin(int amount) {
        if (!machine.IsValidAmount(amount)) {
            Console.WriteLine("Invalid coin! Returning...");
            return;
        }
        Console.WriteLine("Machine is dispensing... Returning " + amount);
    }
}

public class GiveChangeState : State {
    public GiveChangeState(VendingMachine machine) {
        base.SetMachine(machine);
    }
    public void OnEnter() {
        if (machine.GetAmount() > 0) {
            Console.WriteLine("Vending Machine gives you " + machine.GetAmount());
            machine.AddAmount(-machine.GetAmount());
        }
        machine.ChangeState(new InsertCoinState(machine));
    }

    public void OnLeave() {
    }

    public override void InsertCoin(int amount) {
        if (!machine.IsValidAmount(amount)) {
            Console.WriteLine("Invalid coin! Returning...");
            return;
        }
        Console.WriteLine("Machine is giving change... Returning " + amount);
    }
}

public class VendingMachine {

    public VendingMachine() {
        this.ChangeState(new InsertCoinState(this));
    }
    private State currentState;
    private int currentAmount = 0;
    private int itemPrice = 50;
    private int itemStock = 5;
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
        if (this.currentState != null) { this.currentState.OnLeave(); }
        this.currentState = newState;
        this.currentState.OnEnter();
    }

}

