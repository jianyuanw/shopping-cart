function initStars(starsDivList) {
    for (let i = 0; i < starsDivList.length; i++) {
        let elements = starsDivList[i].children;
        let stars = starsDivList[i].getAttribute("data-average");
        if (stars == null) {
            stars = 0;
        }
        highlightStars(elements, stars);
    }
}

function ratingStars(event) {
    let myClass = event.target.getAttribute("class");
    // Checks to ensure that the user has highlighted one of the stars, as opposed to other parts of urdiv
    if (myClass == null || !myClass.startsWith("stars")) {
        return;
    }

    let elements = document.getElementsByClassName(myClass);
    let productId = event.target.parentElement.getAttribute("data-product");
    let stars = event.target.parentElement.getAttribute("data-rated");

    if (stars === null || stars === '0') {
        let data = event.target.getAttribute("name");
        stars = data.slice(-1);
        document.getElementById('rating' + productId).innerHTML = "Rate this:"
    } else {
        event.target.removeAttribute("style");
        document.getElementById('rating' + productId).innerHTML = "Your rating:"
    }

    stars = parseInt(stars);

    prevEventTarget = event.target;
    
    highlightStars(elements, stars);
}

function averageStars(event) {
    let myClass = event.target.getAttribute("class");
    // Checks to ensure that the user has highlighted one of the stars, as opposed to other parts of urdiv
    if (myClass == null || !myClass.startsWith("stars")) {
        return;
    }

    let elements = document.getElementsByClassName(prevEventTarget.getAttribute("class"));
    let productId = event.target.parentElement.getAttribute("data-product");
    let stars = prevEventTarget.parentElement.getAttribute("data-average");

    if (stars == null) { stars = 0; }
    document.getElementById('rating' + productId).innerHTML = "Average rating:"
    highlightStars(elements, stars)
}

function newStar(event) {
    let myClass = event.target.getAttribute("class");
    // Checks to ensure that the user has highlighted one of the stars, as opposed to other parts of urdiv
    if (myClass == null || !myClass.startsWith("stars")) {
        return;
    }

    let parent = event.target.parentElement;
    if (parent.getAttribute("data-rated") !== '0') {
        return;
    }

    let elements = document.getElementsByClassName(myClass);
    let productId = event.target.parentElement.getAttribute("data-product");
    let data = event.target.getAttribute("name");
    let stars = parseInt(data.slice(-1));

    ajaxRequestRating(productId, stars);
    parent.setAttribute("data-rated", String(stars));
    highlightStars(elements, stars);
}

function highlightStars(elements, stars) {
    let half = 0;
    if (stars.length > 1) {
        half = stars.slice(-1);
        stars = parseInt(stars.slice(0, 1));
    }

    for (let i = 0; i < elements.length; i++) {
        let currentStar = elements[i].getAttribute("name")
        if (currentStar == null) {
            continue;
        } else {
            currentStar = parseInt(currentStar.substr(4));
        }


        if (currentStar <= stars) {
            elements[i].src = "/images/full_star.png";
        } else if (currentStar === stars + 1 && half === '5') {
            elements[i].src = "/images/half_star.png";
        } else {
            elements[i].src = "/images/blank_star.png";
        }
    }
}

function ajaxRequestRating(productId, rating) {
    let xhr = new XMLHttpRequest();

    xhr.open("POST", "/Gallery/Rate");
    xhr.setRequestHeader("Content-Type", "application/x-www-form-urlencoded", "charset=utf8");

    xhr.onreadystatechange = function () {
        if (this.readyState === XMLHttpRequest.DONE) {
            // receives response from server
            if (this.status == 200) {
                let data = JSON.parse(this.responseText);
                if (data.success === true) {
                    // Updates the specific product average rating and your rating once successful
                    document.getElementById('urdiv' + productId).setAttribute('data-average', data.newAverage.toString());
                } else if (data.redirect) {
                    // Redirects to location returned in json if it fails and redirect is present
                    window.location.href = data.redirect
                }
            }
        }
    };

    // send key value pairs to server
    xhr.send('productId=' + productId + "&rating=" + rating);
}