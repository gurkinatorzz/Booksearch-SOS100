//Random book cover-bild för varje bok.
const bookCover = document.querySelectorAll(".bookCover");
let books = [
    "/images/bookImages/book.png",
    "/images/bookImages/book2.png",
    "/images/bookImages/book3.png",
    "/images/bookImages/book4.png",
    "/images/bookImages/book5.png",
    "/images/bookImages/book6.png"
];

bookCover.forEach(function(cover) {
    let rnd = Math.floor(Math.random() * books.length);
    cover.src = books[rnd];
});
/*--------------------------------------------------------------------------------*/

//VISA ALLA BÖCKER expand: (toggle button)
//bookRentalContainer
const showAllBtn = document.getElementById("showAllBtn"); //"Visa alla böcker"-knapp
const bookContainer = document.querySelector(".bookRentalContainer"); //container som ska expanderas

let expanded = false;
showAllBtn.addEventListener("click", () => {
    
    if (!expanded) {
        //ändrar höjden
        bookContainer.style.overflowY = "visible";
        bookContainer.style.maxHeight = "99999999999999px";
        showAllBtn.textContent = "Visa mindre";
        bookContainer.style.maskImage = "none";
    }

    else {
        bookContainer.style.overflowY = "hidden";
        bookContainer.style.maxHeight = "50vh";
        bookContainer.style.maskImage = "linear-gradient(to bottom, black 85%, transparent 100%)";
        showAllBtn.textContent = "Visa alla";
        
    }
    
    expanded = !expanded;
});