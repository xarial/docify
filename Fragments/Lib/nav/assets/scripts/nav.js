function OpenMenu() {
    var x = document.getElementById("top-menu");
    if (x.className === "menu sub-menu-item-container") {
        x.className += " responsive";
    } else {
        x.className = "menu sub-menu-item-container";
    }
}