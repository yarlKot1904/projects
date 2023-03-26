#include "base-types.h"
#include "Shape.h"
#include <iostream>

double rectangle:: getArea() { return len_ * height_; };

rectangle& rectangle::clone() {
	rectangle* newRectangle = new rectangle(*this);
	return *newRectangle;
};

void rectangle::move(double delta_x, double delta_y) {
	center_.setXY(center_.getX() + delta_x, center_.getY() + delta_y);
}
void rectangle::moveTo(double x, double y) {
	center_.setXY(x, y);
}

void rectangle::scale(double x_, double y_, double sigma) {
	double x = (x_ - getX()) * sigma;
	double y = (y_ - getY()) * sigma;
	moveTo(x, y);
	height_ *= sigma;
	len_ *= sigma;
}

std::string rectangle::getName() {
	return "RECTANGLE";
}
rectangle& rectangle::getFrameRect() {
	return clone();
}

double rectangle::getMaxWidth() {
	return len_;
}
double rectangle::getMaxHeigth() {
	return height_;
}

double rectangle::getPoints(int index) {
	switch (index)
	{
	case 0:
		return center_.getX() - (len_ / 2);
		break;
	case 1:
		return center_.getX() + (len_ / 2);
		break;
	case 2:
		return center_.getY() - (height_ / 2);
		break;
	case 3:
		return center_.getY() + (height_ / 2);
		break;
	default:
		stderr -2;
		return -1;
	}
}

