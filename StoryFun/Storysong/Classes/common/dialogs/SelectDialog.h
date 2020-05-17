#ifndef _SELECT_DIALOG_H_
#define _SELECT_DIALOG_H_

#include "cocos2d.h"
#include "KGDialog.h"

USING_NS_CC;
using namespace ui;

class SelectDialogStart : public KGDialog
{
public:
	DIALOG_CREATE_FUNC(SelectDialogStart);
	void initDialog(DialogSelectListener * listener);
};

class SelectDialogFinish : public KGDialog
{
public:
	DIALOG_CREATE_FUNC(SelectDialogFinish);
	void initDialog(DialogSelectListener * listener);
};

class ListenFinishedDialog : public KGDialog
{
public:
	DIALOG_CREATE_FUNC(ListenFinishedDialog);
	void initDialog(DialogSelectListener * listener);
};


class ListenStartDialog : public KGDialog
{
public:
	DIALOG_CREATE_FUNC(ListenStartDialog);
	void initDialog(DialogSelectListener * listener);

};



class HelpDialog : public KGDialog
{
public:
	DIALOG_CREATE_FUNC(SelectDialogFinish);
	void initDialog(DialogSelectListener * listener);

};

class LoadingDialog : public KGDialog
{
public:
	DIALOG_CREATE_FUNC(SelectDialogFinish);
	void initDialog();

};

class DimLayer : public KGDialog
{
public:
	DIALOG_CREATE_FUNC(DimLayer);
	void initDialog();
};


#endif