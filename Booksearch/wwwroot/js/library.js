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
