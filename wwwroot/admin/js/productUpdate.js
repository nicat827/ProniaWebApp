
const deleteBtns = document.querySelectorAll(".cus-delete-btn")

if (deleteBtns) {
    deleteBtns.forEach(btn => {
        btn.addEventListener("click", (e) => {
            btn.parentElement.remove();
        })
    })
}