window.onload = function () {
    let elemList = document.getElementsByClassName("productQuantity");

    for (let i = 0; i < elemList.length; i++) {
        elemList[i].addEventListener("input", updateQuantity);
    }
}

function updateQuantity(event) {
    let cartItem = event.target;
    let cartId = cartItem.getAttribute("id");
    let productId = parseInt(cartId.substring(cartId.indexOf("quantity") + 8));
    ajaxRequest(productId, parseInt(cartItem.value));
}

function ajaxRequest(productId, quantity) {
    let xhr = new XMLHttpRequest();

    xhr.open("POST", "/Cart/Update");
    xhr.setRequestHeader("Content-Type", "application/x-www-form-urlencoded", "charset=utf8");

    xhr.onreadystatechange = function ()
    {
        if (this.readyState === XMLHttpRequest.DONE)
        {
            // receives response from server
            if (this.status == 200)
            {
                let data = JSON.parse(this.responseText);
                if (data.success === true)
                {
                    document.getElementById('price' + productId).innerHTML = "Price: $" + data.newPrice;                    
                    document.getElementById('totalPrice').innerHTML = "Total: $" + data.totalPrice;
                }
            }
        }
    };

    // send key value pairs to server
    xhr.send('productId=' + productId + "&quantity=" + quantity);
}