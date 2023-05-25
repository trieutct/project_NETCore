const next = document.querySelector('.next');
const prev = document.querySelector('.prev');
const comment = document.querySelector('#list-comment');
const commentItem = document.querySelectorAll('#list-comment .item');
var translateY = 0;
var count = commentItem.length;


console.log(next);
console.log(prev);
console.log(comment);
console.log(commentItem);
console.log(count);
prev.addEventListener('click', function (event) {
    event.preventDefault()
    if (count == commentItem.length) {
        //xem het binh luan
        return false
    }
    translateY += 400
    comment.style.transform = `translateY(${translateY}px)`;
    count++;
});
next.addEventListener('click', function (event) {
    event.preventDefault()
    if (count == 1) {
        //xem het binh luan
        return false
    }
    translateY += -400
    comment.style.transform = `translateY(${translateY}px)`;
    count--;
})
