//
//  CacaoLogin.m
//  CapcomWorld
//
//  Created by yongho Kim on 12. 11. 2..
//
//

//#include <stdio.h>

#import "KAAuth.h"
#import "KakaoAuthLoginViewController.h"
#import "KakaoAuthMainViewController.h"





static NSString *const kClientSecret = @"mQBOSCzOvub+conITjarmsj9vDPI+drfA7ZJpRp6JR5rmkimtC18kPPVpKLWcqOUAZaYX3YQajyxTM87uLaNTA==";

static NSString *const kClientID = @"88543098824610336";
static NSString *const kRedirectURL = @"kakao88543098824610336://exec";

static NSString *const kAccessTokenKey = @"accessToken";
static NSString *const kRefreshTokenKey = @"refreshToken";

//@implementation AppDelegate
//@synthesize mainViewController = _mainViewController;

//@interface AppDelegate () <KakaoAuthLoginViewControllerDelegate, KakaoAuthMainViewControllerDelegate>

//@implementation AppDelegate

//KakaoAuthLoginViewControllerDelegate *loginViewDelegate;


void KAKAO_Setup()
{
    
    KAAuth *kakao = [[KAAuth alloc] initWithClientID:@"88543098824610336"
                                        clientSecret:@"mQBOSCzOvub+conITjarmsj9vDPI+drfA7ZJpRp6JR5rmkimtC18kPPVpKLWcqOUAZaYX3YQajyxTM87uLaNTA=="
                                         redirectURL:@"kakao88543098824610336://exec"
                                         accessToken:kAccessTokenKey
                                        refreshToken:kRefreshTokenKey];
    [KAAuth setSharedAuth:kakao];
    [kakao release];
}

/*
void showLoginView()
{
    NSString *nibName = nil;
    if ([[UIDevice currentDevice] userInterfaceIdiom] == UIUserInterfaceIdiomPhone) {
        nibName = @"KakaoAuthLoginViewController_iPhone";
    } else {
        nibName = @"KakaoAuthLoginViewController_iPad";
    }
    
    KakaoAuthLoginViewController *loginViewController = [[KakaoAuthLoginViewController alloc] initWithNibName:nibName bundle:nil];
    loginViewController.delegate = self;
    self.mainViewController = loginViewController;
    self.window.rootViewController = loginViewController;
    [loginViewController release];
    [self.window makeKeyAndVisible];
}
*/
/*
void showAuthTestView()
{
    NSString *nibName = nil;
    if ([[UIDevice currentDevice] userInterfaceIdiom] == UIUserInterfaceIdiomPhone) {
        nibName = @"KakaoAuthMainViewController_iPhone";
    } else {
        nibName = @"KakaoAuthMainViewController_iPad";
    }
    
    KakaoAuthMainViewController *authTestViewController = [[KakaoAuthMainViewController alloc] initWithNibName:nibName
                                                                                                        bundle:nil];
    authTestViewController.delegate = self;
    UINavigationController *navigationController = [[UINavigationController alloc] initWithRootViewController:authTestViewController];
    self.mainViewController = navigationController;
    self.window.rootViewController = navigationController;
    [authTestViewController release];
    [navigationController release];
    [self.window makeKeyAndVisible];
}
*/

