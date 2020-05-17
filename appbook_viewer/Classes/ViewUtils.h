#pragma once
#include "cocos2d.h"

//#define STAGE_WIDTH 1024
//#define STAGE_HEIGHT 768

using namespace cocos2d;

class CViewUtils
{
public:
	CViewUtils();
	~CViewUtils();

public:
	static float STAGE_HEIGHT;
	static float STAGE_WIDTH;

	/*�̹��� ������ ũ�⿡ ���� ������ ����*/
	static void setSize(Node* node, float x, float y, float width, float height) {

		Node* _node = node;
		_node->setScaleX(1.0f);
		_node->setScaleY(1.0f);
		auto size = _node->getContentSize();

		float _scaleX = width / size.width;
		float _scaleY = height / size.height;
		_node->setScaleX(_scaleX);
		_node->setScaleY(_scaleY);

		setPosition(_node, x, y);
	}

	static void setSizeByScale(Node* node, float x, float y) {

		Node* _node = node;
		_node->setScaleX(1.0f);
		_node->setScaleY(1.0f);
		auto size = _node->getContentSize();

		float _scaleX = 1280.0f / 1920.0f;// size.width;
		float _scaleY = 800.0f / 1200.0f;// size.height;
		_node->setScaleX(_scaleX);
		_node->setScaleY(_scaleY);

		setPosition(_node, x, y);
	}

	// 1920 * 1200 ������ ��ǥ �� ����� 1280 * 800 �������� ��ȯ�Ͽ� �׸���. 
	static void setPosAndSizeByScale(Node* node, float x, float y) {

		Node* _node = node;
		_node->setScaleX(1.0f);
		_node->setScaleY(1.0f);
		auto size = _node->getContentSize();

		float _scaleX = 1280.0f / 1920.0f;// size.width;
		float _scaleY = 800.0f / 1200.0f;// size.height;
		_node->setScaleX(_scaleX);
		_node->setScaleY(_scaleY);

		_node->setPosition(Vec2(x * _scaleX, y * _scaleY));
	}

	/*��ġ ����*/
	static void setPosition(Node* node, float x, float y) {
		Node* _node = node;

		float _scaleX = _node->getScaleX();
		float _scaleY = _node->getScaleY();

		Size size = _node->getContentSize();

		float width = size.width*_scaleX;
		float height = size.height*_scaleY;

		float _x = (width / 2) + x;
		float _y = (STAGE_HEIGHT - (height / 2)) - y;

		_node->setPosition(Vec2(_x, _y));
	}
};

