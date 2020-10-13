window.onload = function () {
    let elemList = document.getElementsByClassName("productQuantity");

    for (let i = 0; i < elemList.length; i++) {
        elemList[i].addEventListener("change", updateProduct);
    }

    let removeList = document.getElementsByClassName("productRemove");

    for (let i = 0; i < removeList.length; i++) {
        removeList[i].addEventListener("click", removeProduct)
    }
}

function updateProduct(event) {
    let cartItem = event.target;

    if (cartItem.value === '' || cartItem.value <= 0) {
        cartItem.value = 1;
    } else if (cartItem.value > 100) {
        cartItem.value = 100;
    }

    let cartId = cartItem.getAttribute("id");
    let productId = parseInt(cartId.substring(cartId.indexOf("quantity") + 8));
    let quantity = parseInt(cartItem.value);

    ajaxRequestUpdate(productId, quantity);
}

function removeProduct(event) {
    let cartItem = event.target;
    let cartId = cartItem.getAttribute("id");
    let productId = parseInt(cartId.substring(cartId.indexOf("remove") + 6));
    let row = cartItem.getAttribute("data-row");

    ajaxRequestRemove(productId, row);
}

function ajaxRequestUpdate(productId, quantity) {
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
                    // Commented out because updating prices dynamically was not fitting specifications
                    //document.getElementById('price' + productId).innerHTML = "$" + data.newPrice;                    
                    document.getElementById('totalPrice').innerHTML = "Total: $" + data.totalPrice;
                }
            }
        }
    };

    // send key value pairs to server
    xhr.send('productId=' + productId + "&quantity=" + quantity);
}

function ajaxRequestRemove(productId, row) {
    let xhr = new XMLHttpRequest();

    xhr.open("POST", "/Cart/Remove");
    xhr.setRequestHeader("Content-Type", "application/x-www-form-urlencoded", "charset=utf8");

    xhr.onreadystatechange = function () {
        if (this.readyState === XMLHttpRequest.DONE) {
            // receives response from server
            if (this.status == 200) {
                let data = JSON.parse(this.responseText);
                if (data.success === true) {
                    // To be done if request is successful
                    document.getElementById("row" + row.toString()).remove();
                    document.getElementById('totalPrice').innerHTML = "Total: $" + data.totalPrice;
                }
            }
        }
    };

    // send key value pairs to server
    xhr.send('productId=' + productId + "&row=" + row);
}