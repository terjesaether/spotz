$(function() {

            var addCommentButton = $('#addCommentBtn');
            var currentUserId = '@User.Identity.GetUserId()';
            var isLoggedIn = @User.Identity.IsAuthenticated.ToString().ToLower();
            //var currentUserName = '@User.Identity.Name';
            var spotzId = '@Model.Spotz.SpotzId';
            var commentField = document.getElementById('addCommentText');
            var descriptionText = $('.spotz-description p');
            var spotzTextfieldWrapper = $('#editDescriptionText');
            var textNewDescription = $('#editDescriptionText textarea');
            //var updateTitleBtn = $('#updateTitleBtn');
            var updateTitleWrapper = $('#updateTitleWrapper');
            var textNewTitle = $('#updateTitleWrapper input');
            var titleText = $('#spotzTitle');
            var textAddTag = $('#textAddTag');

            var isOwner = function() {
                if (currentUserId === '@Model.Spotz.User.Id') {
                    return true;
            }
                return false;
            }
        


        textNewDescription.val(descriptionText.text());
        textNewTitle.val(titleText.text());
        

        // KNOCKOUT
        function ViewModel() {
            var self = this;
            self.tags = ko.observableArray([]);
            self.images = ko.observableArray([]);
            //self.showTagDeleteBtn = isOwner();
            //self.showImageDeleteBtn = isOwner();
            self.isOwner = isOwner();

            self.removeTagKo = function(tag) {
                self.tags.remove(this);               
                removeTag(tag.TagId);
            };
            self.addTagKo = function () {                
                var newKoTag = self.tags;
                addTag(newKoTag);
            };

            self.deleteImageKo = function (image) {
                self.images.remove(this);              
                deleteImage(image.ImageId);
            }

            console.log('Getting images');            
            $.get('/api/getimages/' + spotzId)          
                .done(function (data) {
                    self.images.push.apply(self.images, data);
                    
                }).fail(function (err) {
                    console.error(err);
                });

            console.log('Getting tags');            
            $.get('/api/TagsApi/' + spotzId)          
                .done(function (data) {
                    self.tags.push.apply(self.tags, data);
                    
                }).fail(function (err) {
                    console.error(err);
                });

        };
        ko.applyBindings(new ViewModel());

        

        $('#editDescriptionToggle').click(function() {
            toggleDescription();
        });

        $('#updateDescriptionBtn').click(function() {
            var url = '/api/UpdateDescriptionText/';
            var data = {
                Id: spotzId,
                Text: textNewDescription.val()
            };
            updateText(url, data, descriptionText, toggleDescription());

        });

        $('#updateTitleBtn').click(function() {
            var url = '/api/UpdateTitleText/';
            var data = {
                Id: spotzId,
                Text: textNewTitle.val()
            };
            updateText(url, data, titleText, toggleTitle());
        });

        $('#editTitleToggle').click(function() {
            toggleTitle();
        });

        //$('.delete-image').click(function() {
        //    var id = $(this).attr('id');
        //    deleteImage(id);
        //});

        // Fjernes?
        //$('#addTagbtn').click(function() {
        //    addTag();
        //});

        // Fjernes? Og fjerne data-tagid?
        //$('.remove-tag-btn').click(function () {
        //    var tagId = $(this).data('tagid');
        //    removeTag(tagId);
        //});

        function toggleTitle() {
            updateTitleWrapper.toggleClass('hidden');
            titleText.toggleClass('hidden');
        }

        function toggleDescription() {
            descriptionText.toggleClass('hidden');
            spotzTextfieldWrapper.toggleClass('hidden');
        }


        //File Upload response from the server
        Dropzone.options.dropzoneForm = {
            autoProcessQueue: false,
            uploadMultiple: false,
            maxFiles: 1,
            addRemoveLinks: true,
            parallelUploads: 100,

            init: function() {

                var myDropZone = this;

                $('#dropzoneForm').on('submit', uploadFile);

                function uploadFile(e) {

                    e.preventDefault();
                    e.stopPropagation();
                    myDropZone.processQueue();
                }

                this.on("success",
                    function(file, data) {
                        console.log(data.imgurl);

                        //$("#spotz-image").attr("src", data.imgurl);
                        document.getElementById('spotz-image').src = data.imgurl;

                        //var res = eval('(' + data.xhr.responseText + ')');
                        //var res = JSON.parse(data.xhr.responseText);
                    });
            }
        };

        addCommentButton.click(function() {
            if (commentField.value === null || commentField.value === '') {
                return;
            }
            $.post('/api/SpotzApi/AddComment/',
                {
                    Id: spotzId,
                    UserId: currentUserId,
                    Comment: commentField.value
                })
                .done(function(data) {
                    if (data.status === "success") {
                        console.log('Success!');
                        commentField.value = 'Takk for kommentaren!';
                        $('#new-comment-wrapper').html(
                            '<div class=\'row comment-section\'>' +
                            '<div class="col-md-2">' +
                            data.user +
                            "<img src='" +
                            data.gravatarurl +
                            "' class='img-responsive' />" +
                            "</div><div class='col-md-10'>" +
                            data.comment +
                            "</div></div>");
                    }


                }).fail(function(err) {
                    console.error(err);
                });
        });

        function updateText(url, inData, updatedElement, toggle) {
            $.post(url, inData)
                .done(function(data) {
                    console.log(data.status);
                    updatedElement.text(data.text);
                    toggle();
                }).fail(function(err) {
                    console.error(err);
                });
        }

        function deleteImage(id) {
            
            $.post('/api/deleteimage/' + id)
                .done(function(data) {
                    //$('#image_' + data.id).fadeOut(1000);
                    //$('#image_' + data.id).remove();
                    console.log(data.status);

                    //toggle();
                }).fail(function(err) {
                    console.error(err);
                });
        }

        function addTag(newKoTag) {
            //alert("tag adding!");
            $.post('/api/addtag/',
                {
                    Id : spotzId,
                    Text : textAddTag.val()
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

        function removeTag(tagId) {
            //alert("Removing tag" + tagId);
            $.post('/api/removetag/',
                {
                    SpotzId : spotzId,
                    TagId : tagId
                })             
                .done(function (data) {
                    //$('#tagMessage').html('Tag removed!').fadeOut(2000);
                    //$('#tag_' + tagId).fadeOut(1000);
                }).fail(function (err) {
                    console.error(err);
                });

        };

        

        $("#textAddTag").autocomplete({
            source: function(request, response) {
                $.ajax({
                    url: '/Home/AutoComplete/',
                    data: "{ 'prefix': '" + request.term + "'}",
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    success: function(data) {

                        response($.map(data,
                            function(item) {
                                return item;
                            }));
                    },
                    error: function(response) {
                        alert(response.responseText);
                    },
                    failure: function(response) {
                        alert(response.responseText);
                    }
                });
            },
            minLength: 2,
            select: function(event, ui) {
                console.log("Selected: " + ui.item.value);
            }
        });


        });