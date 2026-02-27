
// visar vilken sida som är aktiv, exkluderar logo bilden i navbar
const links = document.querySelectorAll("nav ul li a"); 

links.forEach((link) => {
    if (link.href === window.location.href) {
        link.classList.add("active");
    }
});

document.addEventListener("DOMContentLoaded", () => {
    const burger = document.querySelector(".hamburgerButton");
    const menu = document.querySelector("#navbarList");

    if (!burger || !menu) return;

    burger.addEventListener("click", (e) => {
        e.stopPropagation(); // så click-outside inte stänger direkt
        burger.classList.toggle("is-open");
        menu.classList.toggle("is-open");
    });

    // stäng när man klickar på en länk
    menu.querySelectorAll("a").forEach(a => {
        a.addEventListener("click", () => {
            burger.classList.remove("is-open");
            menu.classList.remove("is-open");
        });
    });

    // stäng när man klickar utanför
    document.addEventListener("click", (e) => {
        if (!menu.contains(e.target) && !burger.contains(e.target)) {
            burger.classList.remove("is-open");
            menu.classList.remove("is-open");
        }
    });
});