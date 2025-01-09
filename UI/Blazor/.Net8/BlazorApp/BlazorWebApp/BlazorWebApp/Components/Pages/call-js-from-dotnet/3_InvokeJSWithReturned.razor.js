export function displayTickerAlert2 (symbol, price) {
    if (price < 20) {
        alert(`${symbol}: $${price}!`);
        return "Alert!";
    } else {
        return "No Alert";
    }
};