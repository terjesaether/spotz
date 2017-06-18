$("#textAddTag").autocomplete({
    source: function (request, response) {
        $.ajax({
            url: '/Home/AutoComplete/',
            data: "{ 'prefix': '" + request.term + "'}",
            dataType: "json",
            type: "POST",
            contentType: "application/json; charset=utf-8",
            success: function (data) {

                response($.map(data,
                    function (item) {
                        return item;
                    }));
            },
            error: function (response) {
                alert(response.responseText);
            },
            failure: function (response) {
                alert(response.responseText);
            }
        });
    },
    minLength: 2,
    select: function (event, ui) {
        console.log("Selected: " + ui.item.value);
    }
});

function addTag(newKoTag) {
    //alert("tag adding!");
    $.post('/api/addtag/',
        {
            Id: spotzId,
            Text: textAddTag.val()
        })
        .done(function (data) {
            if (data.status === "tagextists") {
                textAddTag.val('');
                $('#tagMessage').html('Tag Exist!').fadeOut(2000);
                alert("Tag exist");
                return;
            }
            $('#tagMessage').html('Tag added!').fadeOut(2000);
            textAddTag.val('');

            newKoTag.push({ TagName: data.text, TagId: data.tagid });

        }).fail(function (err) {
            console.error(err);
        });

};