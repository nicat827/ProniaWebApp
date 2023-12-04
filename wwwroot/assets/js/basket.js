const inps = document.querySelectorAll("#prod-count-id")


inps.forEach((inp) => {
    
    inp.addEventListener("keyup", (e) => {
        console.log("7777")
        let value = Number(e.target.value)
        let productId = inp.getAttribute("data-id")
        console.log(productId)
        if (value !== 0) {
            fetch(`http://localhost:5270/basket/add/${productId}?ctrl=basket&act=index`)
                .then(res => res.text())
                .then(data => {
                    const start = data.indexOf("<main class=")
                    const end = data.indexOf("</main>")
                    const html = data.slice(start, end)
                    const render = document.querySelector("#rend-div")
                    render.removeChild(document.querySelector("main"))
                    render.innerHTML = html

                })
        }
    })
})
