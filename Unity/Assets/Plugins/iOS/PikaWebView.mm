// 簡単なWebView
#import <UIKit/UIKit.h>

#ifndef NSFoundationVersionNumber_iOS_7_1
#define NSFoundationVersionNumber_iOS_7_1 1047.25
#endif

#define BELOW_IOS_8 (NSFoundationVersionNumber <= NSFoundationVersionNumber_iOS_7_1)

extern UIViewController *UnityGetGLViewController();

extern "C" void UnitySendMessage(const char *, const char *, const char *);

// ローディングマーク
@interface WebViewSpinner : UIView
@property (nonatomic, retain) UIActivityIndicatorView *indicator;
-(id) initFullscreen;
-(void) show;
-(void) hide;
@end

// ローディングマーク実装
@implementation WebViewSpinner
-(id) initFullscreen
{
    CGSize screen = [[UIScreen mainScreen] bounds].size;
    CGRect frame = CGRectMake(0, 0, screen.width, screen.height);
    self = [super initWithFrame:frame];
    if (self)
    {
        self.backgroundColor = [UIColor colorWithRed:0 green:0 blue:0 alpha:0.5];
        self.clipsToBounds = YES;
        self.layer.cornerRadius = 10;
        
        self.indicator = [[UIActivityIndicatorView alloc] initWithFrame:CGRectMake(screen.width/2-32, screen.height/2-32, 64, 64)];
        self.indicator.activityIndicatorViewStyle = UIActivityIndicatorViewStyleWhiteLarge;
        [self addSubview:self.indicator];
    }
    
    return self;
}

-(void) show
{
    self.hidden = NO;
    [self.indicator startAnimating];
}

-(void) hide
{
    self.hidden = YES;
    [self.indicator stopAnimating];
}

@end

// WebView class
@class RopeWebView;
static RopeWebView* _webViewInstance = nil;
static NSString* callbackObjectName = nil;
static NSString* callbackFuncName = nil;
static bool spinnerVisible = true;

@interface RopeWebView : UIWebView<UIWebViewDelegate>
@property (nonatomic, retain) WebViewSpinner* spinner;
+(RopeWebView*) startWithFrame:(CGRect)frame;
-(id) initWithFrame:(CGRect)frame;

@end

// Implementation
@implementation RopeWebView
+(RopeWebView*) startWithFrame:(CGRect)frame
{
    if (nil == _webViewInstance)
    {
        _webViewInstance = [[RopeWebView alloc] initWithFrame:frame];
        _webViewInstance.scalesPageToFit = NO;
        _webViewInstance.mediaPlaybackRequiresUserAction = NO;
    }
    else
    {
        _webViewInstance.hidden = YES;
    }
    return _webViewInstance;
}

-(id) initWithFrame:(CGRect)frame
{
    self = [super initWithFrame:frame];
    if (self)
    {
        _spinner = [[WebViewSpinner alloc] initFullscreen];
        [_spinner hide];
        
        self.delegate = self;
    }
    return self;
}

-(void) webViewDidFinishLoad:(RopeWebView*)webView
{
    [webView.spinner hide];
    webView.backgroundColor = [UIColor clearColor];
    webView.autoresizingMask = UIViewAutoresizingFlexibleHeight;
    [UnityGetGLView() setUserInteractionEnabled:YES];
}

-(void) webViewDidStartLoad:(RopeWebView*)webView
{
    if (spinnerVisible)
        [webView.spinner show];
    [UnityGetGLView() setUserInteractionEnabled:NO];
}

-(void) webView:(RopeWebView *)webView didFailLoadWithError:(NSError *)error
{
    [webView.spinner hide];
    [UnityGetGLView() setUserInteractionEnabled:YES];
    if (nil != callbackObjectName && nil != callbackFuncName)
    {
        UnitySendMessage(callbackObjectName.UTF8String, callbackFuncName.UTF8String, error.description.UTF8String);
    }
}
@end

// 共有関数
extern "C"
{
    void _OpenWebView(const char* name, float x, float y, float w, float h, float r);
    void _OpenWebHtml(const char* name, float x, float y, float w, float h, float r);
    void _CloseWebView();
    void _SetCallback(const char* objectName, const char* funcName);
    void _ShowSpinner(bool visible);
}

void _OpenWebView(const char* name, float x, float y, float w, float h, float r)
{
    // WebViewは一つ
    if (nil != _webViewInstance)
        return;
    
    CGRect screen = [[UIScreen mainScreen] bounds];
    float rate = screen.size.height / r;
    
    float width = w * rate;
    float height = h * rate;
    float left = (screen.size.width - width) / 2 + x * rate;
    float top = (screen.size.height - height) / 2 - y * rate;
    
    RopeWebView* view = [RopeWebView startWithFrame:CGRectMake(left, top, width, height)];
    if (nil != view)
    {
        // Add view to unity
        UIView *unityView = UnityGetGLViewController().view;
        [unityView addSubview:view];
        [unityView addSubview:view.spinner];
        // Request URL
        NSString* strUrl = [NSString stringWithUTF8String:name];
        NSURL* url = [NSURL URLWithString:strUrl];
        NSURLRequest* request = [NSURLRequest requestWithURL:url cachePolicy:NSURLRequestReloadIgnoringLocalCacheData timeoutInterval:60];
        [view loadRequest:request];
    }
}

void _OpenWebHtml(const char* name, float x, float y, float w, float h, float r)
{
    // WebViewは一つ
    if (nil != _webViewInstance)
        return;
    
    CGRect screen = [[UIScreen mainScreen] bounds];
    float rate = screen.size.height / r;
    
    float width = w * rate;
    float height = h * rate;
    float left = (screen.size.width - width) / 2 + x * rate;
    float top = (screen.size.height - height) / 2 - y * rate;
    
    RopeWebView* view = [RopeWebView startWithFrame:CGRectMake(left, top, width, height)];
    if (nil != view)
    {
        // Add view to unity
        UIView *unityView = UnityGetGLViewController().view;
        [unityView addSubview:view];
        [unityView addSubview:view.spinner];
        // Request HTML
        NSString* strHtml = [NSString stringWithUTF8String:name];
        [view loadHTMLString:strHtml baseURL:nil];
    }
}

void _CloseWebView()
{
    if (nil != _webViewInstance)
    {
        // Stop request
        [_webViewInstance stopLoading];
        // Remove view
        _webViewInstance.hidden = YES;
        [_webViewInstance.spinner removeFromSuperview];
        [_webViewInstance removeFromSuperview];
        _webViewInstance = nil;
        [UnityGetGLView() setUserInteractionEnabled:YES];
    }
    
    callbackObjectName = nil;
    callbackFuncName = nil;
}

void _SetCallback(const char* objectName, const char* funcName)
{
    if (!objectName || !funcName)
        return;
    
    callbackObjectName = [NSString stringWithUTF8String:objectName];
    callbackFuncName = [NSString stringWithUTF8String:funcName];
}

void _ShowSpinner(bool visible)
{
    spinnerVisible = visible;
}
