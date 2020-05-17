//
//  HitDetectHelper.h
//
//  
//
//

#ifndef __HitDetectHelper__
#define __HitDetectHelper__

#include "cocos2d.h"

class HitDetectHelper
{
	class HitDetectHelperNode;

private:

	static cocos2d::RenderTexture* _sharedRenderTexture;
	static HitDetectHelperNode* _helperNode;

#pragma mark - Hit Test

public:

	static bool hitTest(cocos2d::Sprite* sprite, const cocos2d::Point& point);

private:

	CC_DISALLOW_IMPLICIT_CONSTRUCTORS(HitDetectHelper)

#pragma mark - HitDetectHelperNode

private:

	class HitDetectHelperNode : public cocos2d::Node
	{

	private:

		cocos2d::CustomCommand _customCommand;
		uint8_t _pixelBuffer[4];

#pragma mark - Properties

		CC_SYNTHESIZE(bool, _pixelRead, PixelRead);
		CC_SYNTHESIZE(cocos2d::Point, _pixelPoint, PixelPoint);

#pragma mark - Reset

	public:

		void reset();

#pragma mark - Pixel Buffer

	public:

		uint8_t getPixelValue(unsigned int index);

#pragma mark - Draw

	public:

		virtual void draw(cocos2d::Renderer *renderer, const cocos2d::Mat4& transform, uint32_t flags) override;

#pragma mark - Read Pixel

	private:

		void readPixel();

#pragma mark - Constructors

	public:

		static HitDetectHelperNode* create(const cocos2d::Point& pixelPoint);

	CC_CONSTRUCTOR_ACCESS:

		HitDetectHelperNode();
		virtual ~HitDetectHelperNode() {};

		bool init(const cocos2d::Point& pixelPoint);

	private:

		CC_DISALLOW_COPY_AND_ASSIGN(HitDetectHelperNode);

	};

};

#endif /* defined(__HitDetectHelper__) */
