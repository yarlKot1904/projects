#include "base-types.h"
#include "Shape.h"
#include "Isotrapezium.h"

double Isotrapezium::getArea() { return (firstWidth_ + secondWidth_) * height_ / 2; };

Isotrapezium& Isotrapezium::clone() {
	Isotrapezium* newFigure = new Isotrapezium(*this);
	return *newFigure;
};

void Isotrapezium::move(double delta_x, double delta_y) {
	center_.setXY(center_.getX() + delta_x, center_.getY() + delta_y);
}
void Isotrapezium::moveTo(double x, double y) {
	center_.setXY(x, y);
}

void Isotrapezium::scale(double x_, double y_, double sigma) {
	double x = (x_ - getX()) * sigma;
	double y = (y_ - getY()) * sigma;
	moveTo(x, y);
	height_ *= sigma;
	firstWidth_ *= sigma;
	secondWidth_ *= sigma;
	max = firstWidth_ > secondWidth_ ? firstWidth_ : secondWidth_;
}

std::string Isotrapezium::getName() {
	return "ISOTRAPEZIUM";
}

rectangle& Isotrapezium::getFrameRect() {
	rectangle* rect = new rectangle(center_, max, height_);
	return *rect;
}

double Isotrapezium::getMaxWidth() {
	return firstWidth_;
}
double Isotrapezium::getMaxHeigth() {
	return height_;
}

double Isotrapezium::getPoints(int index) {
	switch (index)
	{
	case 0:
		return center_.getX() - (max / 2);
		break;
	case 1: return center_.getX() + (max / 2);
		break;
	case 2: return center_.getY() - (height_ / 2);
		break;
	case 3 : return center_.getY() + (height_ / 2);
	default:
		stderr - 1;
		return -1;
		break;
	}
}