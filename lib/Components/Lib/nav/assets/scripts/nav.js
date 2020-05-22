function OpenMenu(toggle) {
    var menu = document.getElementById("top-menu");
    if (menu.className === "menu sub-menu-item-container") {
        menu.className += " responsive";
    } else {
        menu.className = "menu sub-menu-item-container";
    }
    toggle.classList.toggle("change");
}