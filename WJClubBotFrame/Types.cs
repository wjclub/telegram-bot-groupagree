using System.Collections.Generic;

namespace WJClubBotFrame.Types {
	using Newtonsoft.Json;
	using System.ComponentModel;

	public class Response {
		[JsonProperty(PropertyName = "ok",  Required = Required.Always)]
		public bool Ok;
		[JsonProperty(PropertyName = "description",  Required = Required.Default)]
		public string Description;
		[JsonProperty(PropertyName = "result",  Required = Required.Always)]
		public Update[] Result;
		[JsonProperty(PropertyName = "parameters", Required = Required.Default)]
		public ResponseParameters Parameters;
	}

	public class ResponseParameters {
		[JsonProperty(PropertyName = "migrate_to_chat_id", Required = Required.Default)]
		public long MigrateToChatID;
		[JsonProperty(PropertyName = "retry_after", Required = Required.Default)]
		public int RetryAfter;
	}

	public class GetMeResponse {
		[JsonProperty(PropertyName = "ok",  Required = Required.Always)]
		public bool Ok;
		[JsonProperty(PropertyName = "description",  Required = Required.Default)]
		public string Description;
		[JsonProperty(PropertyName = "result",  Required = Required.Always)]
		public User Result;
	}

	public class SendMessageResponse {
		[JsonProperty(PropertyName = "ok",  Required = Required.Always)]
		public bool Ok;
		[JsonProperty(PropertyName = "description",  Required = Required.Default)]
		public string Description;
		[JsonProperty(PropertyName = "result",  Required = Required.Always)]
		public Message Result;
	}

	public class Update {
		[JsonProperty(PropertyName = "update_id",  Required = Required.Always)]
		public int UpdateId;
		[JsonProperty(PropertyName = "message",  Required = Required.Default)]
		public Message Message;
		[JsonProperty(PropertyName = "edited_message",  Required = Required.Default)]
		public Message EditedMessage;
		[JsonProperty(PropertyName = "inline_query",  Required = Required.Default)]
		public InlineQuery InlineQuery;
		[JsonProperty(PropertyName = "chosen_inline_result",  Required = Required.Default)]
		public ChosenInlineResult ChosenInlineResult;
		[JsonProperty(PropertyName = "callback_query",  Required = Required.Default)]
		public CallbackQuery CallbackQuery;
	}

	public class User {
		[JsonProperty(PropertyName = "id",  Required = Required.Always)]
		public int Id;
		[JsonProperty(PropertyName = "first_name",  Required = Required.Always)]
		public string FirstName;
		[JsonProperty(PropertyName = "last_name",  Required = Required.Default)]
		public string LastName;
		[JsonProperty(PropertyName = "username",  Required = Required.Default)]
		public string Username;
		[DefaultValue("none")]
		[JsonProperty(PropertyName = "language_code", Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Populate)]
		public string LanguageCode;
	}

	public class Chat {
		[JsonProperty(PropertyName = "id",  Required = Required.Always)]
		public long Id;
		[JsonProperty(PropertyName = "type",  Required = Required.Always)]
		public string Type;
		[JsonProperty(PropertyName = "title",  Required = Required.Default)]
		public string Title;
		[JsonProperty(PropertyName = "username",  Required = Required.Default)]
		public string Username;
		[JsonProperty(PropertyName = "first_name",  Required = Required.Default)]
		public string FirstName;
		[JsonProperty(PropertyName = "last_name",  Required = Required.Default)]
		public string LastName;
	}

	public class Message {
		[JsonProperty(PropertyName = "message_id",  Required = Required.Always)]
		public int MessageId;
		[JsonProperty(PropertyName = "from",  Required = Required.Default)]
		public User From;
		[JsonProperty(PropertyName = "date",  Required = Required.Always)]
		public int Date;
		[JsonProperty(PropertyName = "chat",  Required = Required.Always)]
		public Chat Chat;
		[JsonProperty(PropertyName = "forward_from",  Required = Required.Default)]
		public User ForwardFrom;
		[JsonProperty(PropertyName = "forward_from_chat",  Required = Required.Default)]
		public Chat ForwardFromChat;
		[JsonProperty(PropertyName = "forward_date",  Required = Required.Default)]
		public int ForwardDate;
		[JsonProperty(PropertyName = "reply_to_message",  Required = Required.Default)]
		public ReplyMessage ReplyToMessage;
		[JsonProperty(PropertyName = "edit_date",  Required = Required.Default)]
		public int EditDate;
		[JsonProperty(PropertyName = "text",  Required = Required.Default)]
		public string Text;
		[JsonProperty(PropertyName = "entities",  Required = Required.Default)]
		public MessageEntity[] Entities;
		[JsonProperty(PropertyName = "audio",  Required = Required.Default)]
		public Audio Audio;
		[JsonProperty(PropertyName = "document",  Required = Required.Default)]
		public Document Document;
		[JsonProperty(PropertyName = "photo",  Required = Required.Default)]
		public PhotoSize [] Photo;
		[JsonProperty(PropertyName = "sticker",  Required = Required.Default)]
		public Sticker Sticker;
		[JsonProperty(PropertyName = "video",  Required = Required.Default)]
		public Video Video;
		[JsonProperty(PropertyName = "voice",  Required = Required.Default)]
		public Voice Voice;
		[JsonProperty(PropertyName = "caption",  Required = Required.Default)]
		public string Caption;
		[JsonProperty(PropertyName = "contact",  Required = Required.Default)]
		public Contact Contact;
		[JsonProperty(PropertyName = "location",  Required = Required.Default)]
		public Location Location;
		[JsonProperty(PropertyName = "venue",  Required = Required.Default)]
		public Venue Venue;
		[JsonProperty(PropertyName = "new_chat_member",  Required = Required.Default)]
		public User NewChatMember;
		[JsonProperty(PropertyName = "left_chat_member",  Required = Required.Default)]
		public User LeftChatMember;
		[JsonProperty(PropertyName = "new_chat_title",  Required = Required.Default)]
		public string NewChatTitle;
		[JsonProperty(PropertyName = "new_chat_photo",  Required = Required.Default)]
		public PhotoSize[] NewChatPhoto;
		[JsonProperty(PropertyName = "delete_chat_photo",  Required = Required.Default)]
		public bool DeleteChatPhoto;
		[JsonProperty(PropertyName = "group_chat_created",  Required = Required.Default)]
		public bool GroupChatCreated;
		[JsonProperty(PropertyName = "migrate_to_chat_id",  Required = Required.Default)]
		public long MigrateToChatId;
		[JsonProperty(PropertyName = "migrate_from_chat_id",  Required = Required.Default)]
		public long MigrateFromChatId;
		[JsonProperty(PropertyName = "pinned_message",  Required = Required.Default)]
		public ReplyMessage PinnedMessage;
	}

	public class ReplyMessage {
		[JsonProperty(PropertyName = "message_id",  Required = Required.Always)]
		public int MessageId;
		[JsonProperty(PropertyName = "from",  Required = Required.Default)]
		public User From;
		[JsonProperty(PropertyName = "date",  Required = Required.Always)]
		public int Date;
		[JsonProperty(PropertyName = "chat",  Required = Required.Always)]
		public Chat Chat;
		[JsonProperty(PropertyName = "forward_from",  Required = Required.Default)]
		public User ForwardFrom;
		[JsonProperty(PropertyName = "forward_from_chat",  Required = Required.Default)]
		public Chat ForwardFromChat;
		[JsonProperty(PropertyName = "forward_date",  Required = Required.Default)]
		public int ForwardDate;
		[JsonProperty(PropertyName = "edit_date",  Required = Required.Default)]
		public int EditDate;
		[JsonProperty(PropertyName = "text",  Required = Required.Default)]
		public string Text;
		[JsonProperty(PropertyName = "entities",  Required = Required.Default)]
		public MessageEntity [] Entities;
		[JsonProperty(PropertyName = "audio",  Required = Required.Default)]
		public Audio Audio;
		[JsonProperty(PropertyName = "document",  Required = Required.Default)]
		public Document Document;
		[JsonProperty(PropertyName = "photo",  Required = Required.Default)]
		public PhotoSize [] Photo;
		[JsonProperty(PropertyName = "sticker",  Required = Required.Default)]
		public Sticker Sticker;
		[JsonProperty(PropertyName = "video",  Required = Required.Default)]
		public Video Video;
		[JsonProperty(PropertyName = "voice",  Required = Required.Default)]
		public Voice Voice;
		[JsonProperty(PropertyName = "caption",  Required = Required.Default)]
		public string Caption;
		[JsonProperty(PropertyName = "contact",  Required = Required.Default)]
		public Contact Contact;
		[JsonProperty(PropertyName = "location",  Required = Required.Default)]
		public Location Location;
		[JsonProperty(PropertyName = "venue",  Required = Required.Default)]
		public Venue Venue;
		[JsonProperty(PropertyName = "new_chat_member",  Required = Required.Default)]
		public User NewChatMember;
		[JsonProperty(PropertyName = "left_chat_member",  Required = Required.Default)]
		public User LeftChatMember;
		[JsonProperty(PropertyName = "new_chat_title",  Required = Required.Default)]
		public string NewChatTitle;
		[JsonProperty(PropertyName = "new_chat_photo",  Required = Required.Default)]
		public PhotoSize[] NewChatPhoto;
		[JsonProperty(PropertyName = "delete_chat_photo",  Required = Required.Default)]
		public bool DeleteChatPhoto;
		[JsonProperty(PropertyName = "group_chat_created",  Required = Required.Default)]
		public bool GroupChatCreated;
		[JsonProperty(PropertyName = "supergroup_chat_created",  Required = Required.Default)]
		public bool SupergroupChatCreated;
		[JsonProperty(PropertyName = "channel_chat_created",  Required = Required.Default)]
		public bool ChannelChatCreated;
		[JsonProperty(PropertyName = "migrate_to_chat_id",  Required = Required.Default)]
		public long MigrateToChatId;
		[JsonProperty(PropertyName = "migrate_from_chat_id",  Required = Required.Default)]
		public long MigrateFromChatId;
	}

	public enum EEntityType {
		mention, hashtag, bot_command, url, email , bold, italic, code, pre, text_link, text_mention, cashtag, phone_number
	}

	public class MessageEntity {
		[JsonProperty(PropertyName = "type",  Required = Required.Always)]
		public EEntityType Type;
		[JsonProperty(PropertyName = "offset",  Required = Required.Always)]
		public int Offset;
		[JsonProperty(PropertyName = "length",  Required = Required.Always)]
		public int Lenght;
		[JsonProperty(PropertyName = "url",  Required = Required.Default)]
		public string Url;
		[JsonProperty(PropertyName = "user",  Required = Required.Default)]
		public User User;
	}

	public class PhotoSize{
		[JsonProperty(PropertyName = "file_id",  Required = Required.Always)]
		public string FileId;
		[JsonProperty(PropertyName = "width",  Required = Required.Always)]
		public int Width;
		[JsonProperty(PropertyName = "height",  Required = Required.Always)]
		public int Height;
		[JsonProperty(PropertyName = "file_size",  Required = Required.Default)]
		public int FileSize;
	}

	public class Audio {
		[JsonProperty(PropertyName = "file_id",  Required = Required.Always)]
		public string FileId;
		[JsonProperty(PropertyName = "duration",  Required = Required.Always)]
		public int Duration;
		[JsonProperty(PropertyName = "performer",  Required = Required.Default)]
		public string Performer;
		[JsonProperty(PropertyName = "title",  Required = Required.Default)]
		public string Title;
		[JsonProperty(PropertyName = "mime_type",  Required = Required.Default)]
		public string MimeType;
		[JsonProperty(PropertyName = "file_size",  Required = Required.Default)]
		public int FileSize;
	}

	public class Document {
		[JsonProperty(PropertyName = "file_id",  Required = Required.Always)]
		public string FileId;
		[JsonProperty(PropertyName = "thumb",  Required = Required.Default)]
		public PhotoSize Thumb;
		[JsonProperty(PropertyName = "file_name",  Required = Required.Default)]
		public string FileName;
		[JsonProperty(PropertyName = "mime_type",  Required = Required.Default)]
		public string MimeType;
		[JsonProperty(PropertyName = "file_size",  Required = Required.Default)]
		public int FileSize;
	}

	public class Sticker {
		[JsonProperty(PropertyName = "file_id",  Required = Required.Always)]
		public string FileId;
		[JsonProperty(PropertyName = "width",  Required = Required.Always)]
		public int Width;
		[JsonProperty(PropertyName = "height",  Required = Required.Always)]
		public int Height;
		[JsonProperty(PropertyName = "thumb",  Required = Required.Default)]
		public PhotoSize Thumb;
		[JsonProperty(PropertyName = "emoji",  Required = Required.Default)]
		public string Emoji;
		[JsonProperty(PropertyName = "file_size",  Required = Required.Default)]
		public int FileSize;
	}

	public class Video {
		[JsonProperty(PropertyName = "file_id",  Required = Required.Always)]
		public string FileId;
		[JsonProperty(PropertyName = "width",  Required = Required.Always)]
		public int Width;
		[JsonProperty(PropertyName = "height",  Required = Required.Always)]
		public int Height;
		[JsonProperty(PropertyName = "duration",  Required = Required.Always)]
		public int Duration;
		[JsonProperty(PropertyName = "thumb",  Required = Required.Default)]
		public PhotoSize Thumb;
		[JsonProperty(PropertyName = "mime_type",  Required = Required.Default)]
		public string MimeType;
		[JsonProperty(PropertyName = "file_size",  Required = Required.Default)]
		public int FileSize;
	}

	public class Voice {
		[JsonProperty(PropertyName = "file_id",  Required = Required.Always)]
		public string FileId;
		[JsonProperty(PropertyName = "duration",  Required = Required.Always)]
		public int Duration;
		[JsonProperty(PropertyName = "mime_type",  Required = Required.Default)]
		public string MimeType;
		[JsonProperty(PropertyName = "file_size",  Required = Required.Default)]
		public int FileSize;
	}

	public class Contact {
		[JsonProperty(PropertyName = "phone_number",  Required = Required.Always)]
		public string PhoneNumber;
		[JsonProperty(PropertyName = "first_name",  Required = Required.Always)]
		public string FirstName;
		[JsonProperty(PropertyName = "last_name",  Required = Required.Default)]
		public string LastName;
		[JsonProperty(PropertyName = "user_id",  Required = Required.Default)]
		public int UserId;
	}

	public class Location {
		[JsonProperty(PropertyName = "longitude",  Required = Required.Always)]
		public float Longitude;
		[JsonProperty(PropertyName = "latitude",  Required = Required.Always)]
		public float Latitude;
	}

	public class Venue {
		[JsonProperty(PropertyName = "location",  Required = Required.Always)]
		public Location Location;
		[JsonProperty(PropertyName = "title",  Required = Required.Always)]
		public string Title;
		[JsonProperty(PropertyName = "address",  Required = Required.Always)]
		public string Address;
		[JsonProperty(PropertyName = "foursquare_id",  Required = Required.Default)]
		public string FoursquareId;
	}

	public class UserProfilePhotos {
		[JsonProperty(PropertyName = "total_count",  Required = Required.Always)]
		public int TotalCount;
		[JsonProperty(PropertyName = "photos",  Required = Required.Always)]
		public PhotoSize[][] Photos;
	}

	public class File {
		[JsonProperty(PropertyName = "file_id",  Required = Required.Always)]
		public string FileId;
		[JsonProperty(PropertyName = "file_size",  Required = Required.Default)]
		public int FileSize;
		[JsonProperty(PropertyName = "file_path",  Required = Required.Default)]
		public string FilePath;
	}

	public abstract class ReplyMarkup {
		
	}

	public class ReplyKeyboardMarkup : ReplyMarkup {
		[JsonProperty(PropertyName = "keyboard",  Required = Required.Always)]
		public List<List<KeyboardButton>> Keyboard;
		[JsonProperty(PropertyName = "resize_keyboard",  Required = Required.Default)]
		public bool ResizeKeyboard;
		[JsonProperty(PropertyName = "one_time_keyboard",  Required = Required.Default)]
		public bool OneTimeKeyboard;
		[JsonProperty(PropertyName = "selective",  Required = Required.Default)]
		public bool Selective;
	}

	public class KeyboardButton {
		[JsonProperty(PropertyName = "text",  Required = Required.Always)]
		public string Text;
		[JsonProperty(PropertyName = "request_contact",  Required = Required.Default)]
		public bool RequestContact;
		[JsonProperty(PropertyName = "request_location",  Required = Required.Default)]
		public bool RequestLocation;
	}

	public class ReplyKeyboardHide : ReplyMarkup {
		[JsonProperty(PropertyName = "hide_keyboard",  Required = Required.Always)]
		public bool HideKeyboard;
		[JsonProperty(PropertyName = "selective",  Required = Required.Default)]
		public bool Selective;
	}

	public class InlineKeyboardMarkup : ReplyMarkup {
		[JsonProperty(PropertyName = "inline_keyboard",  Required = Required.Always)]
		public List<List<InlineKeyboardButton>> InlineKeyboard;
	}

	public class InlineKeyboardButton {
		public InlineKeyboardButton (string text, string url, string callbackData, string switchInlineQuery) {
			this.Text = text;
			this.Url = url;
			this.CallbackData = callbackData;
			this.SwitchInlineQuery = switchInlineQuery;
		}
		public static InlineKeyboardButton Create(string text, string url = null, string callbackData = null, string switchInlineQuery = null) {
			return new InlineKeyboardButton (text, url, callbackData, switchInlineQuery);
		}
		[JsonProperty(PropertyName = "text",  Required = Required.Always)]
		public string Text;
		[JsonProperty(PropertyName = "url",  Required = Required.Default)]
		public string Url;
		[JsonProperty(PropertyName = "callback_data",  Required = Required.Default)]
		public string CallbackData;
		[JsonProperty(PropertyName = "switch_inline_query",  Required = Required.Default)]
		public string SwitchInlineQuery;
	}

	public class CallbackQuery {
		[JsonProperty(PropertyName = "id",  Required = Required.Always)]
		public string Id;
		[JsonProperty(PropertyName = "from",  Required = Required.Always)]
		public User From;
		[JsonProperty(PropertyName = "message",  Required = Required.Default)]
		public Message Message;
		[JsonProperty(PropertyName = "inline_message_id",  Required = Required.Default)]
		public string InlineMessageId;
		[JsonProperty(PropertyName = "data",  Required = Required.Always)]
		public string Data;
	}

	public class ForceReply : ReplyMarkup {
		[JsonProperty(PropertyName = "force_reply",  Required = Required.Always)]
		public bool Force_Reply;
		[JsonProperty(PropertyName = "selective",  Required = Required.Default)]
		public bool Selective;
	}

	public class ChatMember {
		[JsonProperty(PropertyName = "user",  Required = Required.Always)]
		public User User;
		[JsonProperty(PropertyName = "status",  Required = Required.Always)]
		public string Status;
	}

	public class InlineQuery {
		[JsonProperty(PropertyName = "id",  Required = Required.Always)]
		public string Id;
		[JsonProperty(PropertyName = "from",  Required = Required.Always)]
		public User From;
		[JsonProperty(PropertyName = "location",  Required = Required.Default)]
		public Location Location;
		[JsonProperty(PropertyName = "query", Required = Required.Always)]
		public string Query { get => HtmlSpecialChars.Decode(query); set => query = value; }
		private string query;
		[JsonProperty(PropertyName = "offset",  Required = Required.Always)]
		public string Offset;
	}

	public abstract class InputMessageContent {

	}

	public class InputTextMessageContent : InputMessageContent {
		public static InputTextMessageContent Create(string messageText, string parseMode = "HTML", bool disableWebPagePreview = false) {
			return new InputTextMessageContent(messageText, parseMode, disableWebPagePreview);
		}
		public InputTextMessageContent(string messageText, string parseMode, bool disableWebPagePreview) {
			this.MessageText = messageText;
			this.ParseMode = parseMode;
			this.DisableWebPagePreview = disableWebPagePreview;
		}
		[JsonProperty(PropertyName = "message_text",  Required = Required.Always)]
		public string MessageText;
		[JsonProperty(PropertyName = "parse_mode",  Required = Required.Default)]
		public string ParseMode;
		[JsonProperty(PropertyName = "disable_web_page_preview",  Required = Required.Default)]
		public bool DisableWebPagePreview;
	}


	public class InputLocationMessageContent : InputMessageContent {
		[JsonProperty(PropertyName = "latitude",  Required = Required.Always)]
		public float Latitude;
		[JsonProperty(PropertyName = "longitude",  Required = Required.Always)]
		public float Longitude;
	}


	public class InputVenueMessageContent : InputMessageContent {
		[JsonProperty(PropertyName = "latitude",  Required = Required.Always)]
		public float Latitude;
		[JsonProperty(PropertyName = "longitude",  Required = Required.Always)]
		public float Longitude;
		[JsonProperty(PropertyName = "title",  Required = Required.Always)]
		public string Title;
		[JsonProperty(PropertyName = "address",  Required = Required.Always)]
		public string Address;
		[JsonProperty(PropertyName = "foursquare_id",  Required = Required.Default)]
		public string FoursquareID;    
	}

	public class InputContactMessageContent : InputMessageContent {
		[JsonProperty(PropertyName = "phone_number",  Required = Required.Always)]
		public string PhoneNumber;
		[JsonProperty(PropertyName = "first_name",  Required = Required.Always)]
		public string FirstName;
		[JsonProperty(PropertyName = "last_name",  Required = Required.Default)]
		public string LastName;    
	}

	public class ChosenInlineResult {
		[JsonProperty(PropertyName = "result_id",  Required = Required.Always)]
		public string ResultId;
		[JsonProperty(PropertyName = "from",  Required = Required.Always)]
		public User From;
		[JsonProperty(PropertyName = "query", Required = Required.Always)]
		public string Query { get => HtmlSpecialChars.Decode(query); set => query = value; }
		private string query;
		[JsonProperty(PropertyName = "location",  Required = Required.Default)]
		public Location Location;
		[JsonProperty(PropertyName = "inline_message_id",  Required = Required.Default)]
		public string InlineMessageId;
	}  

	public abstract class InlineQueryResult {
		[JsonProperty(PropertyName = "id",  Required = Required.Always)]
		public string Id;
		[JsonProperty(PropertyName = "type",  Required = Required.Always)]
		public EInlineQueryResultTypes Type;
	}

	public class InlineQueryResultArticle : InlineQueryResult {
		public static InlineQueryResultArticle Create(string id, string title, InputMessageContent inputMessageContent, InlineKeyboardMarkup replyMarkup = null, string url = null, bool hideUrl = false, string description = null, string thumbUrl = null, int? thumbWidth = null, int? thumbHeight = null) {
			return new InlineQueryResultArticle (id, title, inputMessageContent, replyMarkup, url, hideUrl, description, thumbUrl, thumbWidth, thumbHeight);
		}
		public InlineQueryResultArticle (string id, string title, InputMessageContent inputMessageContent, InlineKeyboardMarkup replyMarkup, string url, bool hideUrl, string description, string thumbUrl, int? thumbWidth, int? thumbHeight) {
			this.Type = EInlineQueryResultTypes.article;
			this.Id = id;
			this.Title = title;
			this.InputMessageContent = inputMessageContent;
			this.ReplyMarkup = replyMarkup;
			this.Url = url;
			this.HideUrl = hideUrl;
			this.Description = description;
			this.ThumbUrl = thumbUrl;
			this.ThumbWidth = thumbWidth;
			this.ThumbHeight = thumbHeight;
		}
		[JsonProperty(PropertyName = "title",  Required = Required.Always)]
		public string Title;
		[JsonProperty(PropertyName = "input_message_content",  Required = Required.Always)]
		public InputMessageContent InputMessageContent;
		[JsonProperty(PropertyName = "reply_markup",  Required = Required.Default)]
		public InlineKeyboardMarkup ReplyMarkup;
		[JsonProperty(PropertyName = "url",  Required = Required.Default)]
		public string Url;
		[JsonProperty(PropertyName = "hide_url",  Required = Required.Default)]
		public bool HideUrl;
		[JsonProperty(PropertyName = "description",  Required = Required.Default)]
		public string Description;
		[JsonProperty(PropertyName = "thumb_url",  Required = Required.Default)]
		public string ThumbUrl;
		[JsonProperty(PropertyName = "thumb_width",  Required = Required.Default)]
		public int? ThumbWidth;
		[JsonProperty(PropertyName = "thumb_height",  Required = Required.Default)]
		public int? ThumbHeight;
	}

	public class InlineQueryResultPhoto  : InlineQueryResult {
		[JsonProperty(PropertyName = "photo_url",  Required = Required.Always, NullValueHandling=NullValueHandling.Include)]
		public string PgotoUrl;
		[JsonProperty(PropertyName = "thumb_url",  Required = Required.Always, NullValueHandling=NullValueHandling.Include)]
		public string ThumbUrl;
		[JsonProperty(PropertyName = "photo_width",  Required = Required.Default, NullValueHandling=NullValueHandling.Include)]
		public int PhotoWidth;
		[JsonProperty(PropertyName = "photo_height",  Required = Required.Default, NullValueHandling=NullValueHandling.Include)]
		public int PhotoHeight;    
		[JsonProperty(PropertyName = "title",  Required = Required.Default, NullValueHandling=NullValueHandling.Include)]
		public string Title;
		[JsonProperty(PropertyName = "description",  Required = Required.Default, NullValueHandling=NullValueHandling.Include)]
		public string Description;
		[JsonProperty(PropertyName = "caption",  Required = Required.Default, NullValueHandling=NullValueHandling.Include)]
		public string Caption;
		[JsonProperty(PropertyName = "reply_markup",  Required = Required.Default, NullValueHandling=NullValueHandling.Include)]
		public InlineKeyboardMarkup ReplyMarkup;
		[JsonProperty(PropertyName = "input_message_content",  Required = Required.Default, NullValueHandling=NullValueHandling.Include)]
		public InputMessageContent InputMessageContent;
	}


	public class InlineQueryResultGif  : InlineQueryResult {
		[JsonProperty(PropertyName = "photo_url",  Required = Required.Always, NullValueHandling=NullValueHandling.Include)]
		public string PgotoUrl;
		[JsonProperty(PropertyName = "thumb_url",  Required = Required.Always, NullValueHandling=NullValueHandling.Include)]
		public string ThumbUrl;
		[JsonProperty(PropertyName = "gif_url",  Required = Required.Always, NullValueHandling=NullValueHandling.Include)]
		public string GifUrl;

		[JsonProperty(PropertyName = "gif_width",  Required = Required.Default, NullValueHandling=NullValueHandling.Include)]
		public int GifWidth;
		[JsonProperty(PropertyName = "gif_height",  Required = Required.Default, NullValueHandling=NullValueHandling.Include)]
		public int GifHeight;
		[JsonProperty(PropertyName = "title",  Required = Required.Default, NullValueHandling=NullValueHandling.Include)]
		public string Title;
		[JsonProperty(PropertyName = "caption",  Required = Required.Default, NullValueHandling=NullValueHandling.Include)]
		public string Caption;
		[JsonProperty(PropertyName = "reply_markup",  Required = Required.Default, NullValueHandling=NullValueHandling.Include)]
		public InlineKeyboardMarkup ReplyMarkup;
		[JsonProperty(PropertyName = "input_message_content",  Required = Required.Default, NullValueHandling=NullValueHandling.Include)]
		public InputMessageContent InputMessageContent;
	}

	public class InlineQueryResultMpeg4Gif  : InlineQueryResult {
		[JsonProperty(PropertyName = "photo_url",  Required = Required.Always, NullValueHandling=NullValueHandling.Include)]
		public string PgotoUrl;
		[JsonProperty(PropertyName = "thumb_url",  Required = Required.Always, NullValueHandling=NullValueHandling.Include)]
		public string ThumbUrl;
		[JsonProperty(PropertyName = "mpeg4_url",  Required = Required.Always, NullValueHandling=NullValueHandling.Include)]
		public string Mpeg4Url;

		[JsonProperty(PropertyName = "mpeg4_width",  Required = Required.Default, NullValueHandling=NullValueHandling.Include)]
		public int Mpeg4Width;
		[JsonProperty(PropertyName = "mpeg4_height",  Required = Required.Default, NullValueHandling=NullValueHandling.Include)]
		public int Mpeg4Height;
		[JsonProperty(PropertyName = "title",  Required = Required.Default, NullValueHandling=NullValueHandling.Include)]
		public string Title;
		[JsonProperty(PropertyName = "caption",  Required = Required.Default, NullValueHandling=NullValueHandling.Include)]
		public string Caption;
		[JsonProperty(PropertyName = "reply_markup",  Required = Required.Default, NullValueHandling=NullValueHandling.Include)]
		public InlineKeyboardMarkup ReplyMarkup;
		[JsonProperty(PropertyName = "input_message_content",  Required = Required.Default, NullValueHandling=NullValueHandling.Include)]
		public InputMessageContent InputMessageContent;
	}

	public class InlineQueryResultVideo  : InlineQueryResult {
		[JsonProperty(PropertyName = "Video_url",  Required = Required.Always, NullValueHandling=NullValueHandling.Include)]
		public string VideoUrl;
		[JsonProperty(PropertyName = "mime_type",  Required = Required.Always, NullValueHandling=NullValueHandling.Include)]
		public string MimeType;
		[JsonProperty(PropertyName = "thumb_url",  Required = Required.Always, NullValueHandling=NullValueHandling.Include)]
		public string ThumbUrl;
		[JsonProperty(PropertyName = "title",  Required = Required.Default, NullValueHandling=NullValueHandling.Include)]
		public string Title;
		[JsonProperty(PropertyName = "caption",  Required = Required.Default, NullValueHandling=NullValueHandling.Include)]
		public string Caption;
		[JsonProperty(PropertyName = "video_width",  Required = Required.Default, NullValueHandling=NullValueHandling.Include)]
		public int VideoWidth;
		[JsonProperty(PropertyName = "video_height",  Required = Required.Default, NullValueHandling=NullValueHandling.Include)]
		public int VideoHeight;    
		[JsonProperty(PropertyName = "video_duration",  Required = Required.Default, NullValueHandling=NullValueHandling.Include)]
		public int VideoDuration;  
		[JsonProperty(PropertyName = "description",  Required = Required.Default, NullValueHandling=NullValueHandling.Include)]
		public string Description;
		[JsonProperty(PropertyName = "reply_markup",  Required = Required.Default, NullValueHandling=NullValueHandling.Include)]
		public InlineKeyboardMarkup ReplyMarkup;
		[JsonProperty(PropertyName = "input_message_content",  Required = Required.Default, NullValueHandling=NullValueHandling.Include)]
		public InputMessageContent InputMessageContent;
	}

	public class InlineQueryResultAudio  : InlineQueryResult {
		[JsonProperty(PropertyName = "audio_url",  Required = Required.Always, NullValueHandling=NullValueHandling.Include)]
		public string AudioUrl;
		[JsonProperty(PropertyName = "title",  Required = Required.Always, NullValueHandling=NullValueHandling.Include)]
		public string Title;
		[JsonProperty(PropertyName = "performer",  Required = Required.Default, NullValueHandling=NullValueHandling.Include)]
		public string Performer;
		[JsonProperty(PropertyName = "audio_duration",  Required = Required.Default, NullValueHandling=NullValueHandling.Include)]
		public int AudioDuration;
		[JsonProperty(PropertyName = "reply_markup",  Required = Required.Default, NullValueHandling=NullValueHandling.Include)]
		public InlineKeyboardMarkup ReplyMarkup;
		[JsonProperty(PropertyName = "input_message_content",  Required = Required.Default, NullValueHandling=NullValueHandling.Include)]
		public InputMessageContent InputMessageContent;
	}

	public class InlineQueryResultVoice  : InlineQueryResult {
		[JsonProperty(PropertyName = "voice_url",  Required = Required.Always, NullValueHandling=NullValueHandling.Include)]
		public string VoiceUrl;
		[JsonProperty(PropertyName = "title",  Required = Required.Always, NullValueHandling=NullValueHandling.Include)]
		public string Title;
		[JsonProperty(PropertyName = "voice_duration",  Required = Required.Default, NullValueHandling=NullValueHandling.Include)]
		public int VoiceDuration;
		[JsonProperty(PropertyName = "reply_markup",  Required = Required.Default, NullValueHandling=NullValueHandling.Include)]
		public InlineKeyboardMarkup ReplyMarkup;
		[JsonProperty(PropertyName = "input_message_content",  Required = Required.Default, NullValueHandling=NullValueHandling.Include)]
		public InputMessageContent InputMessageContent;
	}


	public class InlineQueryResultDocument  : InlineQueryResult {
		[JsonProperty(PropertyName = "document_url",  Required = Required.Always, NullValueHandling=NullValueHandling.Include)]
		public string DocumentUrl;
		[JsonProperty(PropertyName = "title",  Required = Required.Always, NullValueHandling=NullValueHandling.Include)]
		public string Title;
		[JsonProperty(PropertyName = "mime_type",  Required = Required.Always, NullValueHandling=NullValueHandling.Include)]
		public string MimeType;
		[JsonProperty(PropertyName = "caption",  Required = Required.Default, NullValueHandling=NullValueHandling.Include)]
		public string Caption;
		[JsonProperty(PropertyName = "description",  Required = Required.Default, NullValueHandling=NullValueHandling.Include)]
		public string Description;
		[JsonProperty(PropertyName = "thumb_url",  Required = Required.Default, NullValueHandling=NullValueHandling.Include)]
		public string ThumbUrl;
		[JsonProperty(PropertyName = "thumb_width",  Required = Required.Default, NullValueHandling=NullValueHandling.Include)]
		public int ThumbWidth;
		[JsonProperty(PropertyName = "thumb_height",  Required = Required.Default, NullValueHandling=NullValueHandling.Include)]
		public int ThumbHeight;
		[JsonProperty(PropertyName = "reply_markup",  Required = Required.Default, NullValueHandling=NullValueHandling.Include)]
		public InlineKeyboardMarkup ReplyMarkup;
		[JsonProperty(PropertyName = "input_message_content",  Required = Required.Default, NullValueHandling=NullValueHandling.Include)]
		public InputMessageContent InputMessageContent;
	}

	public class InlineQueryResultLocation  : InlineQueryResult {
		[JsonProperty(PropertyName = "title",  Required = Required.Always, NullValueHandling=NullValueHandling.Include)]
		public string Title;
		[JsonProperty(PropertyName = "longitude",  Required = Required.Always, NullValueHandling=NullValueHandling.Include)]
		public float Longitude;
		[JsonProperty(PropertyName = "latitude",  Required = Required.Always, NullValueHandling=NullValueHandling.Include)]
		public float Latitude;
		[JsonProperty(PropertyName = "thumb_url",  Required = Required.Default, NullValueHandling=NullValueHandling.Include)]
		public string ThumbUrl;
		[JsonProperty(PropertyName = "thumb_width",  Required = Required.Default, NullValueHandling=NullValueHandling.Include)]
		public int ThumbWidth;
		[JsonProperty(PropertyName = "thumb_height",  Required = Required.Default, NullValueHandling=NullValueHandling.Include)]
		public int ThumbHeight;
		[JsonProperty(PropertyName = "reply_markup",  Required = Required.Default, NullValueHandling=NullValueHandling.Include)]
		public InlineKeyboardMarkup ReplyMarkup;
		[JsonProperty(PropertyName = "input_message_content",  Required = Required.Default, NullValueHandling=NullValueHandling.Include)]
		public InputMessageContent InputMessageContent;
	}

	public class InlineQueryResultVenue  : InlineQueryResult {
		[JsonProperty(PropertyName = "title",  Required = Required.Always, NullValueHandling=NullValueHandling.Include)]
		public string Title;
		[JsonProperty(PropertyName = "longitude",  Required = Required.Always, NullValueHandling=NullValueHandling.Include)]
		public float Longitude;
		[JsonProperty(PropertyName = "latitude",  Required = Required.Always, NullValueHandling=NullValueHandling.Include)]
		public float Latitude;
		[JsonProperty(PropertyName = "address",  Required = Required.Always, NullValueHandling=NullValueHandling.Include)]
		public string Address;
		[JsonProperty(PropertyName = "foursquare_id",  Required = Required.Default, NullValueHandling=NullValueHandling.Include)]
		public string FoursquareId;
		[JsonProperty(PropertyName = "thumb_url",  Required = Required.Default, NullValueHandling=NullValueHandling.Include)]
		public string ThumbUrl;
		[JsonProperty(PropertyName = "thumb_width",  Required = Required.Default, NullValueHandling=NullValueHandling.Include)]
		public int ThumbWidth;
		[JsonProperty(PropertyName = "thumb_height",  Required = Required.Default, NullValueHandling=NullValueHandling.Include)]
		public int ThumbHeight;
		[JsonProperty(PropertyName = "reply_markup",  Required = Required.Default, NullValueHandling=NullValueHandling.Include)]
		public InlineKeyboardMarkup ReplyMarkup;
		[JsonProperty(PropertyName = "input_message_content",  Required = Required.Default, NullValueHandling=NullValueHandling.Include)]
		public InputMessageContent InputMessageContent;
	}

	public class InlineQueryResultContact  : InlineQueryResult {
		[JsonProperty(PropertyName = "phone_number",  Required = Required.Always, NullValueHandling=NullValueHandling.Include)]
		public string PhoneNumber;
		[JsonProperty(PropertyName = "first_name",  Required = Required.Always, NullValueHandling=NullValueHandling.Include)]
		public string FirstName;
		[JsonProperty(PropertyName = "last_name",  Required = Required.Default, NullValueHandling=NullValueHandling.Include)]
		public string LastName;
		[JsonProperty(PropertyName = "thumb_url",  Required = Required.Default, NullValueHandling=NullValueHandling.Include)]
		public string ThumbUrl;
		[JsonProperty(PropertyName = "thumb_width",  Required = Required.Default, NullValueHandling=NullValueHandling.Include)]
		public int ThumbWidth;
		[JsonProperty(PropertyName = "thumb_height",  Required = Required.Default, NullValueHandling=NullValueHandling.Include)]
		public int ThumbHeight;
		[JsonProperty(PropertyName = "reply_markup",  Required = Required.Default, NullValueHandling=NullValueHandling.Include)]
		public InlineKeyboardMarkup ReplyMarkup;
		[JsonProperty(PropertyName = "input_message_content",  Required = Required.Default, NullValueHandling=NullValueHandling.Include)]
		public InputMessageContent InputMessageContent;
	}
}

