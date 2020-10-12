window.onload = function () {
    let codeSelectorList = document.getElementsByClassName("code-selector");

    for (let i = 0; i < codeSelectorList.length; i++) {
        codeSelectorList[i].addEventListener("change", updatePurchaseDate);
    }
}

function updatePurchaseDate(event) {
    let selectedCode = event.currentTarget.value;
    //alert("You have selected activation code: " + selectedCode);
    let xhr = new XMLHttpRequest();
    xhr.open("POST", "/Purchase/UpdatePurchaseDate");
    xhr.setRequestHeader("Content-Type", "application/json");
    xhr.onreadystatechange = function () {
        if (this.readyState === XMLHttpRequest.DONE) {
            if (this.status === 200) {
                let data = JSON.parse(this.responseText);
                let dateElementId = "product-" + data.productId;
                let purchaseDate = data.purchaseDate;
                document.getElementById(dateElementId).innerHTML = purchaseDate;
            }
        }
    }
    xhr.send(JSON.stringify(selectedCode));
}
