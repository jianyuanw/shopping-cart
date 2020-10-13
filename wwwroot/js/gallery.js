let prevEventTarget;

window.onload = function () {
    let elemList = document.getElementsByClassName("productbtn");

    for (let i = 0; i < elemList.length; i++) {
        elemList[i].addEventListener("click", onclick);
    }

    let starsDivList = document.getElementsByClassName("urdiv");

    initStars(starsDivList);

    for (let i = 0; i < starsDivList.length; i++) {
        starsDivList[i].addEventListener("mouseover", ratingStars);
        starsDivList[i].addEventListener("mouseout", averageStars);
        starsDivList[i].addEventListener("click", newStar);
    }
}

function onclick(event) {
    let elem = event.currentTarget;
    let proId = elem.getAttribute("productId");

    let xhr = new XMLHttpRequest();

    xhr.open("POST", "/Gallery/addCart");
    xhr.setRequestHeader("Content-Type", "application/x-www-form-urlencoded", "charset=utf8");
    xhr.onreadystatechange = function () {
        if (this.readyState === XMLHttpRequest.DONE) {
            // receives response from server
            if (this.status == 200) {
                let data = JSON.parse(this.responseText);
                if (data.success === true) {
                    let cartelem = document.getElementById('cartnum');
                    let num = cartelem.innerHTML;
                    cartelem.innerHTML = parseInt(num) + 1;
                } else {
                    // Reloads to parse the alert during failure
                    window.location.reload();
                    return false;
                }
            }
        }
    };

    // send like/unlike choice to server
    xhr.send('productId=' + proId);
}