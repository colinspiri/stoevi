using System;
using UnityEngine;

public class DaySummaryUI : MonoBehaviour {
    // components
    public SummaryLineUI savingsLine;
    public SummaryLineUI harvestLine;
    public SummaryLineUI expensesLine;
    public SummaryLineUI totalLine;
    
    // SO
    [Space] 
    public IntReference currentBalance;
    public IntReference playerTomatoes;
    public FloatReference tomatoValue;
    public IntReference dailyExpenses;

    private void OnEnable() {
        int total = 0;
        
        savingsLine.description = "savings";
        savingsLine.value = currentBalance.Value;
        savingsLine.UpdateUI();
        total += savingsLine.value;

        harvestLine.description = "tomatoes (" + playerTomatoes.Value + ")";
        harvestLine.value = (int)Mathf.Round(playerTomatoes.Value * tomatoValue.Value);
        harvestLine.UpdateUI();
        total += harvestLine.value;

        expensesLine.description = "expenses";
        expensesLine.value = -dailyExpenses.Value;
        expensesLine.UpdateUI();
        total += expensesLine.value;

        totalLine.value = total;
        totalLine.UpdateUI();
        
        // update current balance
        currentBalance.Variable.SetValue(total);
    }
}
