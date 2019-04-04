package org.ico.token;

import java.math.BigInteger;

import org.neo.smartcontract.framework.Helper;

public class TemplateToken {

	/**
	 * The full name of the token
	 */
	private final static String NAME = "Test Token";

	/**
	 * The symbol that will be used to identify the token
	 */
	private final static String SYMBOL = "TST";

	/**
	 * The unique id to the owner of the token
	 */
	private final static String OWNER = "AK2nJJpJr6o664CWJKi1QRXjqeic2zRp8y";

	/**
	 * The factor is the value of a single token after the factor is applied
	 */
	private final static long FACTOR = 1000000000;

	private final static long NEO_DECIMALS = 1000000000;

	private final static long TOTAL_AMOUNT = 100000000 * FACTOR;

	private final static long PRE_ICO_CAP = 30000000 * FACTOR;

	private final static long BASIC_RATE = 1000 * FACTOR;

	public static String getName() {
		return NAME;
	}

	public static String getSymbol() {
		return SYMBOL;
	}

	public static byte[] getOwner() {
		return Helper.asByteArray(OWNER);
	}

	/**
	 * 
	 * Because NEO is traded in whole numbers we set a decimal value so that
	 * smaller amounts of the token can be traded. 1 token = 1*10^8 = 100000000
	 */
	public static byte decimals() {
		return 8;
	}

	/**
	 * The neo decimals is the value of a single token after the factor is
	 * applied
	 */
	public static BigInteger getDecimals() {
		return BigInteger.valueOf(NEO_DECIMALS);
	}

	/**
	 * The total number of token that exist
	 */
	public static BigInteger getTotalAmount() {
		return BigInteger.valueOf(TOTAL_AMOUNT);
	}

	public static BigInteger getPreIcoCap() {
		return BigInteger.valueOf(PRE_ICO_CAP);
	}

	/**
	 * The current exchange rate between the ICO tokens and neo during the token
	 * swap period
	 */
	public static BigInteger getBasicRate() {
		return BigInteger.valueOf(BASIC_RATE);
	}

}
