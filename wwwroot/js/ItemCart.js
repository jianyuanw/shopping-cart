window.onload = function () {
    generateSubTotal();
}

function generateSubTotal() {
    var cart_table = document.getElementById("cart_table");
    var items_types = cart_table.rows.length - 1;
    var total = 0;
    for (var i = 0; i < items_types; i++) {
        var items_price = Number(document.getElementById("priceID" + i).innerText);
        var items_quantity = Number(document.getElementById("quantityID" + i).innerText);
        var subtotal = (items_price * items_quantity);
        total += subtotal;
        document.getElementById("totalID" + i).innerText = items_price;
    }
    document.getElementById("all_total").innerText = total;
}

//function minusButton() {

//}

//function addButton() {

//}

//function deleteButton() {

//}