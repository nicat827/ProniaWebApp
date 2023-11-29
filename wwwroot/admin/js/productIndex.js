console.log("Hello")

const errDiv = document.querySelector("#err")
console.log(errDiv)
if (errDiv) {
    
    Swal.fire({
        title: "Good job!",
        text: "You clicked the button!",
        icon: "success"
    });
}