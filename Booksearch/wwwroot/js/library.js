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
const searchBar = document.querySelector(".searchBar");
const sortBy = document.querySelector(".sortBy");

let expanded = false;
showAllBtn.addEventListener("click", () => {

    if (!expanded) {
        //ändrar höjden
        bookContainer.style.overflowY = "visible";
        bookContainer.style.maxHeight = "99999999999999px";
        showAllBtn.textContent = "Visa mindre";
        bookContainer.style.maskImage = "none";

        searchBar.style.display = "flex";
        sortBy.style.display = "flex";
        
        const offset = 150; // justera vid behov :)

        const topPosition = bookContainer.getBoundingClientRect().top + window.scrollY - offset;

        window.scrollTo({
            top: topPosition,
            behavior: "smooth"
        });
    }

    else {
        bookContainer.style.overflowY = "hidden";
        bookContainer.style.maxHeight = "50vh";
        bookContainer.style.maskImage = "linear-gradient(to bottom, black 85%, transparent 100%)";
        showAllBtn.textContent = "Visa alla";

        searchBar.style.display = "none";
        sortBy.style.display = "none";
        
        window.scrollTo({
            top: 0,
            behavior: "smooth"
        });
    }

    expanded = !expanded;
});

/*--------------------------------------------------------------------------------*/
//Sök + sorteringsfunktion

const searchInput = document.getElementById("searchInput");
const clearSearchBtn = document.getElementById("clearSearchBtn");
const sortSelect = document.getElementById("sortSelect");
const allBookCards = Array.from(document.querySelectorAll(".book"));

function getBookInfo(book) {
    const title = book.querySelector("h2")?.textContent.trim().toLowerCase() || "";
    const paragraphs = book.querySelectorAll("p");

    const author = paragraphs[0]?.textContent.trim().toLowerCase() || "";
    const category = paragraphs[1]?.textContent.trim().toLowerCase() || "";
    const yearText = paragraphs[2]?.textContent.trim() || "";
    const year = parseInt(yearText) || 0;
    const id = parseInt(book.dataset.id) || 0;

    return {
        id, title, author, category, year
    };
}

function filterBooks(bookList, searchText) {
    const query = searchText.trim().toLowerCase();

    if (query === "") {
        return bookList;
    }

    return bookList.filter((book) => {
        const info = getBookInfo(book);

        return (
            info.title.includes(query) ||
            info.author.includes(query) ||
            info.category.includes(query) ||
            info.year.toString().includes(query)
        );
    });
}

function sortBooks(bookList, sortValue) {
    const sortedBooks = [...bookList];

    sortedBooks.sort((a, b) => {
        const bookA = getBookInfo(a);
        const bookB = getBookInfo(b);

        switch (sortValue) {
            case "latest":
                return bookB.id - bookA.id;

            case "id":
                return bookA.id - bookB.id;

            case "title":
                return bookA.title.localeCompare(bookB.title, "sv");

            case "titleReversed":
                return bookB.title.localeCompare(bookA.title, "sv");

            case "author":
                return bookA.author.localeCompare(bookB.author, "sv");

            case "category":
                return bookA.category.localeCompare(bookB.category, "sv");

            case "year":
                return bookB.year - bookA.year;

            case "yearReversed":
                return bookA.year - bookB.year;

            default:
                return 0;
        }
    });

    return sortedBooks;
}

function renderBooks() {
    const searchValue = searchInput ? searchInput.value : "";
    const sortValue = sortSelect ? sortSelect.value : "latest";

    const filteredBooks = filterBooks(allBookCards, searchValue);
    const sortedBooks = sortBooks(filteredBooks, sortValue);

    bookContainer.innerHTML = "";

    if (sortedBooks.length === 0) {
        bookContainer.innerHTML = "<div><p>Inga böcker matchade din sökning.</p></div>";
        return;
    }

    sortedBooks.forEach(book => {
        bookContainer.appendChild(book);
    });
}

if (searchInput) {
    searchInput.addEventListener("input", renderBooks);
}

if (sortSelect) {
    sortSelect.addEventListener("change", renderBooks);
}

if (clearSearchBtn) {
    clearSearchBtn.addEventListener("click", () => {
        if (searchInput) {
            searchInput.value = "";
        }
        renderBooks();
    });
}